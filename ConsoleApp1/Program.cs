using Npgsql;
using Npgsql.Internal.TypeHandlers.GeometricHandlers;
using Npgsql.Internal.TypeHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    internal class Program
    {
        private static NpgsqlConnection PostSQLcon = new NpgsqlConnection();
        static void Main(string[] args)
        {
            connModel connection=new connModel();

            try
            {
                Console.Write("Ingrese la dirección IP del servidor: ");
                string srvip = Console.ReadLine();
                connection = new connModel(srvip, "postgres", "Ipte123+++", "5432", "");

            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            }
            List<Carril> carriles = new List<Carril>();
            carriles = InitializeApplication(connection);
            Carril carril;


            int opt=99;
            while (opt != 0)
            {
                try
                {
                    Console.WriteLine("____________________________________________________________________");
                    Console.WriteLine("Herramienta de diagnóstico\n\n1-Revisar última transacción en carril\n2-Revisar tiempo entre transacciones\n3-Comprobar conexión\n8-Limpiar consola\n0-Terminar");
                    Console.Write("Seleccione una opción: ");
                    opt = int.Parse(Console.ReadLine());
                    Console.WriteLine("\n__________________________________________________________________");
                    switch (opt)
                    {
                        case 0: break;
                        case 1:
                            carril = PcSelect(carriles);
                            setFormat(ExtractRow(carril));
                            break;
                        case 2:
                            connection.setDb("styx");
                            Connect(PostSQLcon, connection);
                            CalcDelay(PostSQLcon);
                            break;
                        case 3:
                            carril = PcSelect(carriles);
                            PingCheck(carril.getIP());
                            break;
                        case 4:

                            break;

                        case 8:
                            Console.Clear();
                            break;

                        default:
                            Console.WriteLine("Intente de nuevo");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("\nIngrese caracteres válidos");
                }
                
            }
        }

        public static List<Carril> InitArray(NpgsqlConnection npgsqlConnection)
        {
            string sqlStr = "select cfg_key,cfg_value,cve_caseta from tab_equipo_cuerpo tec \r\ninner join tab_parametros tp on tp.ide_equipo_cuerpo = tec.ide_equipo_cuerpo\r\ninner join tab_cuerpos tc on tc.ide_cuerpo  = tec.ide_cuerpo\r\nwhere cfg_key='PCCarril'";
            var cmd = new NpgsqlCommand(sqlStr, npgsqlConnection);
            var carriles = new List<Carril>();
            using (
                var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string nom= reader.GetString(2);
                    string ip=reader.GetString(1);

                    Carril carril = new Carril(nom,ip);
                    carriles.Add(carril);
                }
            }

            foreach(var carril in carriles)
            {
                Console.WriteLine($"Nombre: {carril.getNombre()}, IP: {carril.getIP()}");
            }
            return carriles;
        }

        public static void CheckTransact()
        {

        }

        public static List<Carril> InitializeApplication(connModel conn)
        {
            Console.WriteLine("Conectando con Cerberus");
            conn.setDb("cerberus");
            Connect(PostSQLcon,conn);
            Console.WriteLine("Extrayendo datos de carril");
            List<Carril> carriles = InitArray(PostSQLcon);
            Console.WriteLine("Conectando con Styx");
            conn.setDb("styx");
            Connect(PostSQLcon, conn);
            return carriles;
        }
        public static Carril PcSelect(List<Carril> carriles)
        {
            try
            {
                Console.WriteLine("Lista de carriles");
                foreach (Carril carril in carriles)
                {
                    Console.WriteLine((carriles.IndexOf(carril) + 1) + "-Carril " + carril.getNombre());
                }
                Console.Write("\nSeleccione el carril: ");
                int opt = int.Parse(Console.ReadLine());
                return carriles[opt - 1];

            }
            catch (Exception ex)
            {
                throw ex;
            }
       
        }

        public static void Connect(NpgsqlConnection connection, connModel model)
        {
            try
            {
                connection.Close();
                connection.ConnectionString = "Host="+model.getHost()+";Username="+model.getUser()+";Password="+model.getPassword()+";Port = "+model.getPort()+";Database="+model.getDb();
                connection.Open();
                if(connection!=null)
                {
                    Console.WriteLine("Correcta conexión con "+model.getDb());
                }
                else
                {
                    Console.WriteLine("No se puede conectar con "+model.getHost());
                }
            }catch (Exception er)
            {
                Console.WriteLine("Error de conexión: \n"+er.Message);
            }
        }

        public static string GetFileName(string ip)
        {
            string dir = @"//"+ip+"/CAPUFE/data/paquet";
            Regex format = new Regex(@"data0\d{6}\.paq");
            string ret="";
            try
            {
                //NetworkCredential networkCredential = new NetworkCredential("PCCarril","1234");
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                FileInfo[] fileInfos = dirInfo.GetFiles();
                List<string> lines = new List<string>();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (format.IsMatch(fileInfo.Name))
                    {
                        lines.Add(fileInfo.Name);
                        ret=fileInfo.FullName;
                        Console.Write(ret);
                    }       
                }
            }catch (Exception ex)
            {
                Console.WriteLine("Error"+ex.Message+"\nRevise los permisos de acceso a la base de datos de Cerberus");
            }
            return ret;
        }

        public static string ExtractRow(Carril target)
        {
            string ret = GetFileName(target.getIP());
            Console.WriteLine(ret);
            Regex format = new Regex("402");
            Regex tag = new Regex("IAV");      
            string[] lines = File.ReadAllLines(ret);
                for (int i = lines.Length -1; i>= 0; i--)
                {
                    string line = lines[i];
                    if (format.IsMatch(line) && tag.IsMatch(line))
                    {
                        return line;
                    }
                    else
                    {
                        return null;
                    }
                }
            return null;
        }

        public static void setFormat(string str)
        {
            if (str == null)
            {
                Console.WriteLine("No se ha registrado una ruta de acceso");
            }
            else
            {
                try
                {
                    Registro reg;
                    int ln = str.Length;
                    string[] arr = new string[ln];
                    string nPlaza = "";
                    string nCarril = "";
                    string fecha = "";
                    string hora = "";
                    for (int i = 0; i < ln; i++)
                    {

                        if (i >= 12 && i < 14)
                        {
                            nPlaza += str[i];
                        }
                        else if (i >= 14 && i < 17)
                        {
                            nCarril += str[i];
                        }
                        else if (i >= 17 && i < 25)
                        {
                            fecha += str[i];
                        }
                        else if (i >= 25 && i < 31)
                        {
                            hora += str[i];
                        }

                    }
                    reg = new Registro(nPlaza, nCarril, ValidateDate(fecha), ValidateTime(hora));
                    Console.WriteLine("\nPago con tag\n\nNúmero de plaza: " + reg.getNumPlaza());
                    Console.WriteLine("Número de carril: " + reg.getNumCarril());
                    Console.WriteLine("Fecha: " + reg.getFecha());
                    Console.WriteLine("Hora: " + reg.getHora() + "\n\n");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }             
        }

        public static string ValidateDate(string str)
        {
            string dt = "";
            for(int i=0;i < str.Length;i++)
            {
                dt += str[i];
                if (i == 3)
                {
                    dt += "/";
                }else if (i == 5)
                {
                    dt += "/";
                }
            }
            return dt;
        }

        public static string ValidateTime(string str)
        {
            string hr = "";
            for(int i=0; i < str.Length; i++)
            {
                hr+= str[i];
                if(i == 1)
                {
                    hr += ":";
                }else if (i == 3)
                {
                    hr += ":";
                }
            }
            return hr;
        }

       public static bool CalcDelay(NpgsqlConnection connection)
        {
            try
            {
                string sqlStr = "select min(sys_update), max(sys_update), (max(sys_update)-min(sys_update)) from tab_tag_cobrado ttc where sys_enabled=true";
                var cmd = new NpgsqlCommand(sqlStr, connection);
                TimeSpan Del=new TimeSpan();
                using(var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime minDate=reader.GetDateTime(0);
                        DateTime maxDate = reader.GetDateTime(1);
                        Del = maxDate-minDate;
                        
                        Console.WriteLine("Primer tag:"+minDate.ToString());
                        Console.WriteLine("Último tag:" + maxDate.ToString());
                        Console.WriteLine("\nRetraso:" + Del.Days+" Dias, "+Del.Hours+" Horas, "+Del.Minutes+" Minutos, "+Del.Seconds+" Segundos, "+Del.Milliseconds+" Milisegundos");
                    }
                }

                if(Del.TotalSeconds > 30)
                {
                    Console.WriteLine("\nRetraso muy alto, los tags no están pasando");
                }
                else
                {
                    Console.WriteLine("Correcto");
                }
                //float del = cmd.ExecuteNonQuery();
                /*Console.WriteLine(del);
                if (del > 30)
                {
                    Console.WriteLine("Anomalía detectada");
                    return true;
                }
                else
                {
                    Console.WriteLine(del);
                    Console.WriteLine("Todo correcto");
                    return false;
                }*/
            }
            catch(Exception ex)
            { 
                Console.WriteLine(ex.Message + "\n");            
            }
            return false;
        }


        public static bool PingCheck(string ip)
        {
            Console.WriteLine("Revisando conexión con " + ip);
            Ping ping = new Ping();
            try
            {
                int count = 0;
                PingReply pingReply;
                for(int i = 0; i < 10; i++){
                    pingReply = ping.Send(ip);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        Console.Write("!");
                        count++;
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                if (count == 10)
                {
                    Console.WriteLine("\nCorrecta conexión con "+ip);
                    return true;
                }
                else
                {
                    Console.WriteLine("\nHa fallado la conexión con"+ip);
                    return false;
                }
            }catch(Exception er)
            {
                Console.WriteLine("Error en ejecución: "+er.Message);
                return false;
            }
        }
    }
}
