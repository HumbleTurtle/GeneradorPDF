# GeneradorPDF
        Como usar con Process
        
        static void Inicializar()
        {
            try
            {
                control = new Dictionary<string, string>();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false; //required to redirect standart input/output

                // redirects on your choice
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;

                startInfo.UseShellExecute = false;

                string path = Path.Combine( HostingEnvironment.ApplicationPhysicalPath, "librerias");
                string direccion = Path.Combine(path, "PDFGenerator.exe");
                startInfo.FileName = direccion;
                startInfo.CreateNoWindow = false;
                startInfo.WorkingDirectory = path;

                proceso = new Process();
                proceso.StartInfo = startInfo;
                proceso.Start();

                proceso.BeginOutputReadLine();
                proceso.BeginErrorReadLine();

                proceso.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {

                    Console.WriteLine(e.Data);

                });

                proceso.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {

                    Console.WriteLine(e.Data);
                    if (e.Data.Equals("INICIADO")) { Iniciado = true;  return;  }

                    // Prepend line numbers to each line of the output.
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        Regex patron = new Regex(@"([^\ ]+) ([^\ ]+)", RegexOptions.IgnoreCase);

                        var matches = patron.Match(e.Data);
                        if (matches.Groups.Count >= 3)
                        {
                            string codigo = matches.Groups[1].Value;
                            string resultado = matches.Groups[2].Value;
                            control.Add(codigo, resultado);
                        }
                    }
                });
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void GenerarPDF( string url ) 
        {

            if (proceso == null)
                Inicializar();

            while (!Iniciado)
                Task.Delay(30);

            Guid gu = Guid.NewGuid();
            proceso.StandardInput.WriteLine(gu.ToString() + " http://google.com");

            //Debug.WriteLine(gu.ToString() + " http://google.com")

            while (true)
                try
                {
                    string valor;
                    control.TryGetValue(gu.ToString(), out valor);
                    if( valor != null )
                    {
                        control.Remove(gu.ToString());
                        break;
                    }                      
                    
                }
                catch (Exception ex)
                {
                    Task.Delay(30);
                }
          
            return null;
        }
