namespace PytaniaInzynierka
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Ustawienia konsoli
            ConsoleSettings();

            // Tworzenie elementów głównego menu
            List<string> menu = new List<string>()
            {
                "Dodaj nowe pytanie do tymczasowej kolekcji (lokalne działanie)",
                "Wyświetl pytania z tymczasowej kolekcji (lokalne działanie)",
                "Wyświetl pytania z bazy danych (plik TXT lokalny)",
                "Dopisz nowo dodane pytania z tymczasowej kolekcji do bazy danych (plik TXT lokalny)",
                "Usuń bazę danych (plik TXT lokalny)",
                "Usuń pojedynczy rekord (z pliku TXT lokalnie)",
                "Edycja bazy danych (pliku TXT lokalnego)",
                "Losowanie pytań (z pliku TXT lokalnego)",
                "Synchronizacja bazy danych (pliku TXT lokalnego z dyskiem OneDrive)",
                "Wykonaj kopię zapasową bazy danych (pliku TXT lokalnie)",
                "Przywróć dane z kopii zapasowej (plik zapasowy lokalny)",
                "Zakończ program"
            };

            // Tworzenie słownika z pytaniami dla tymczasowego przechowywania do zapisu do pliku TXT
            Dictionary<string, string> questionsTemporary = new Dictionary<string, string>();

            // Tworzenie słownika z pytaniami do losowania
            Dictionary<string,string> questionsLoterry = new Dictionary<string,string>();

            // Lista kategorii pytań  ***DLA DODAWANIA NOWEJ KATEGORII EDYCJA TUTAJ**
            List<string> categories = new List<string>
            {
                "Wszystkie pytania",
                "Pytania inżynierskie cz.1",
                "Pytania inżynierskie cz.2",
                "Pytania inżynierskie cz.3",
                "Pytania inżynierskie cz.4"
            };

            try
            {
                // Główna pętla programu (wczytuje menu, zarządza wyborami)
                bool programRun = true;
                do
                {
                    int wybor = RunMenu(menu);

                    switch (wybor)
                    {
                        case 0:
                            string question = CreateQuestion(categories);
                            string answer = CreateAnswer(question);
                            AddToDictionary(questionsTemporary, question, answer);
                            break;
                        case 1:
                            DisplayList(questionsTemporary);
                            break;
                        case 2:
                            DisplayTxtFile();
                            break;
                        case 3:
                            AppendToTxtFile(questionsTemporary);
                            break;
                        case 4:
                            DeleteTxtFile();
                            break;
                        case 5:
                            DeleteFromTxtById();
                            break;
                        case 6:
                            EditTxtFile();
                            break;
                        case 7:
                            QuestionsLoterry(questionsLoterry, categories);
                            break;
                        case 8:
                            SyncTxtFile();
                            break;
                        case 9:
                            backupTxtFile();
                            break;
                        case 10:
                            RestoreFromBackup();
                            break;
                        case 11:
                            if (questionsTemporary.Count != 0)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Masz niezapisane dane. Kolekcja tymczasowa wymaga zapisu do pliku TXT. Mimo to wyjść? y/n");
                                Console.ForegroundColor = ConsoleColor.White;

                                string input = Console.ReadLine();
                                if (input == "y")
                                {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("Niezapisane dane zostają utracone. Zamykam program.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    programRun = false;
                                    Environment.Exit(0);
                                }
                                else Console.Clear();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Brak zmian do zapisu. Zamykam program.");
                                Console.ForegroundColor = ConsoleColor.White;
                                programRun = false;
                                Environment.Exit(0);
                            }
                            break;
                    }
                }
                while (programRun == true);
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Wystąpił nieoczekiwany błąd.");
                Console.ForegroundColor = ConsoleColor.White;

                LogToFile(ex);

#if DEBUG
                Console.WriteLine(ex.Message);
                Console.WriteLine("---");
                Console.WriteLine("Źródło wyjątku: " + ex.Source);
                Console.ReadKey();
#endif
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        // LOG
        static void LogToFile(Exception ex)
        {
            string logFile = "ErrorLog.txt";
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine(ex.Message);
            sw.Close();
            sw.Dispose();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Zapisano wystąpienie błędu w pliku {logFile}");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadKey();
        }

        // Przywróć dane z kopii zapasowej
        static void RestoreFromBackup()
        {
            string localFile = "questions.txt";
            string localBackupFile = "questions_backup.txt";

            if(File.Exists(localBackupFile))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Przywrócić dane z kopii zapasowej? y/n");
                Console.ForegroundColor = ConsoleColor.White;

                string input = Console.ReadLine();

                if (input == "y")
                {
                    Dictionary<string, string> questionsFromBackup = new Dictionary<string, string>();
                    string lineBackup = "";
                    string[] lineBackupSplited;

                    StreamReader sr = new StreamReader(localBackupFile);

                    Console.WriteLine("Rozpoczynam pobieranie danych z kopii zapasowej.");

                    do
                    {
                        lineBackup = sr.ReadLine();
                        lineBackupSplited = lineBackup.Split(';');
                        questionsFromBackup.Add(lineBackupSplited[0], lineBackupSplited[1]);
                    }
                    while (!sr.EndOfStream);

                    sr.Close();
                    sr.Dispose();


                    Console.WriteLine("Zapisuję dane z kopii zapasowej do lokalnego pliku.");
                    StreamWriter sw = new StreamWriter(localFile, false);

                    foreach(KeyValuePair<string, string> question in questionsFromBackup)
                    {
                        sw.WriteLine(question.Key + ";" + question.Value);
                    }

                    sw.Close();
                    sw.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Przywrócenie danych poprawne.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Przywrócenie danych anulowane.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                Console.WriteLine("Nie można przywrócić danych. Kopia zapasowa nie istnieje.");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Kopia zapasowa pliku TXT
        static void backupTxtFile()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Czy wykonac kopię zapasową bazy danych (pliku TXT)? y/n");
            Console.ForegroundColor = ConsoleColor.White;

            string input = Console.ReadLine();

            if(input == "y")
            {
                string path = "questions.txt";

                if (File.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Wykonuje kopię zapasową lokalnie");
                    Console.ForegroundColor = ConsoleColor.White;

                    string newPath = "questions_backup.txt";
                    File.Copy(path, newPath, true);

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"kopia zapasowa({newPath}) została utworzona.");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("ENTER-->dalej");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Nie można wykonać kopii zapasowej. Baza danych(plik TXT nie istnieje lokalnie).");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("ENTER-->dalej");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Nie wykonuję kopii zapasowej.");
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("ENTER-->dalej");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Synchronizacja pliku TXT
        static void SyncTxtFile()
        {
            string filePathLocal = "questions.txt";
            string filePathOneDrivePc = "C:\\Users\\alekk\\OneDrive\\Dokumenty\\Database_Pytania_Inzynierka\\questions.txt";
            string filePathOneDriveLaptop = "C:\\Users\\Dell\\OneDrive\\Dokumenty\\Database_Pytania_Inzynierka\\questions.txt";

            string machinePc = "C:\\Users\\alekk";
            string machineLaptop = "C:\\Users\\Dell";

            
            if (File.Exists(filePathOneDrivePc))
            {
                Console.WriteLine("Wykryta maszyna: PC");
                FileInfo fi = new FileInfo(filePathOneDrivePc);
                long fileSize = fi.Length;

                if(fileSize <= 0)
                {
                    Console.WriteLine("Plik istnieje na dysku OneDrive, ale jest pusty.");

                    if (File.Exists(filePathLocal))
                    {
                        Console.WriteLine("Baza danych(plik TXT) istnieje lokalnie. Pobieram jego dane.");
                        StreamReader srLocal = new StreamReader(filePathLocal);
                        Dictionary<string, string> questionsLocal = new Dictionary<string, string>();
                        string lineLocal = "";
                        string[] lineLocalSplited;

                        do
                        {
                            lineLocal = srLocal.ReadLine();
                            lineLocalSplited = lineLocal.Split(';');
                            questionsLocal.Add(lineLocalSplited[0], lineLocalSplited[1]);
                        }
                        while (!srLocal.EndOfStream);

                        srLocal.Close();
                        srLocal.Dispose();

                        Console.WriteLine("Aktualizuję dysk OneDrive. Zapisywanie danych z pliku lokalnego.");

                        StreamWriter sw = new StreamWriter(filePathOneDrivePc, false);

                        foreach (KeyValuePair<string, string> question in questionsLocal)
                        {
                            sw.WriteLine(question.Key + ";" + question.Value);
                        }

                        sw.Close();
                        sw.Dispose();

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Plik TXT na dysku OneDrive został zaktualizowany.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Nie można zaktualizować dysku OneDrive. Brak bazy danych(pliku TXT) lokalnie.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Plik na dysku OneDrive ma pojemność {fileSize} bajtów.");
                    Console.ForegroundColor = ConsoleColor.White;

                    StreamReader sr = new StreamReader(filePathOneDrivePc);
                    Dictionary<string, string> questionsPc = new Dictionary<string, string>();
                    string line = "";
                    string[] lineSplitted;
                    do
                    {
                        line = sr.ReadLine();
                        lineSplitted = line.Split(';');
                        questionsPc.Add(lineSplitted[0], lineSplitted[1]);
                    }
                    while (!sr.EndOfStream);

                    sr.Close();
                    sr.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Plik został pobrany z dysku OneDrive.");
                    Console.WriteLine("Sprawdzam czy plik TXT istnieje lokalnie.");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("ENTER-->kontynuuj");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    Console.Clear();

                    if (File.Exists(filePathLocal))
                    {
                        Console.WriteLine("Baza danych(plik TXT) istnieje lokalnie. Sprawdzam aktualność danych.");
                        StreamReader srLocal = new StreamReader(filePathLocal);
                        Dictionary<string, string> questionsLocal = new Dictionary<string, string>();
                        string lineLocal = "";
                        string[] lineLocalSplited;

                        do
                        {
                            lineLocal = srLocal.ReadLine();
                            lineLocalSplited = lineLocal.Split(';');
                            questionsLocal.Add(lineLocalSplited[0], lineLocalSplited[1]);
                        }
                        while (!srLocal.EndOfStream);

                        srLocal.Close();
                        srLocal.Dispose();

                        int questionsOneDriveCount = questionsPc.Count();
                        int questionsLocalCount = questionsLocal.Count();

                        if (questionsOneDriveCount > questionsLocalCount)
                        {
                            Console.WriteLine($"Ilość pytań w lokalnej bazie({questionsLocalCount}) jest mniejsza niż na dysku OneDrive({questionsOneDriveCount})");
                            Console.WriteLine("Aktualizuję lokalny pik TXT.");

                            StreamWriter sw = new StreamWriter(filePathLocal, false);

                            foreach (KeyValuePair<string, string> question in questionsPc)
                            {
                                sw.WriteLine(question.Key + ";" + question.Value);
                            }

                            sw.Close();
                            sw.Dispose();

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Plik TXT lokalny został zaktualizowany.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (questionsOneDriveCount < questionsLocalCount)
                        {
                            Console.WriteLine($"Ilość pytań w lokalnej bazie({questionsLocalCount}) jest większa niż na dysku OneDrive({questionsOneDriveCount})");
                            Console.WriteLine("Aktualizuję dysk OneDrive.");

                            StreamWriter sw = new StreamWriter(filePathOneDrivePc, false);

                            foreach (KeyValuePair<string, string> question in questionsLocal)
                            {
                                sw.WriteLine(question.Key + ";" + question.Value);
                            }

                            sw.Close();
                            sw.Dispose();

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Plik TXT na dysku OneDrive został zaktualizowany.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine($"Ilość pytań w lokalnej bazie({questionsLocalCount}) jest równa ilośći pytań na dysku OneDrive({questionsOneDriveCount})");

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("Wszystko jest aktualne.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Baza danych(plik TXT) nie istnieje lokalnie.");
                        Console.WriteLine("Tworzę lokalną bazę danych na podstawie pliku z dysku OneDrive.");

                        StreamWriter sw = new StreamWriter(filePathLocal, false);

                        foreach (KeyValuePair<string, string> question in questionsPc)
                        {
                            sw.WriteLine(question.Key + ";" + question.Value);
                        }

                        sw.Close();
                        sw.Dispose();

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Plik TXT lokalny został utworzony.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                        Console.Clear();
                    }
                } 
            }
            else if(!File.Exists(filePathOneDrivePc) && Directory.Exists(machinePc))
            {
                Console.WriteLine("Wykryta maszyna: PC");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Plik TXT na dysku OneDrive nie istnieje.");
                Console.WriteLine("Sprawdzam czy istnieje lokalnie.");
                Console.ForegroundColor = ConsoleColor.White;

                if (File.Exists(filePathLocal))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Znaleziono lokalny plik TXT.");
                    Console.WriteLine("Pobieram dane z lokalnego pliku.");
                    Console.ForegroundColor = ConsoleColor.White;

                    Dictionary<string, string> questionsLocal = new Dictionary<string, string>();
                    StreamReader sr = new StreamReader(filePathLocal);
                    string lineLocal = "";
                    string[] lineLocalSplited;

                    do
                    {
                        lineLocal = sr.ReadLine();
                        lineLocalSplited = lineLocal.Split(';');
                        questionsLocal.Add(lineLocalSplited[0], lineLocalSplited[1]);
                    }
                    while (!sr.EndOfStream);

                    sr.Close();
                    sr.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Zapisuję kopię danych na dysku OneDrive.");
                    Console.ForegroundColor = ConsoleColor.White;

                    StreamWriter sw = new StreamWriter(filePathOneDrivePc, false);

                    foreach (KeyValuePair<string, string> question in questionsLocal)
                    {
                        sw.WriteLine(question.Key + ";" + question.Value);
                    }

                    sw.Close();
                    sw.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Zapis kopii pliku lokalnego na dysku OneDrive zakończony.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Nie można utworzyć kopii na dysku OneDrive. Lokalny plik TXT nie istnieje.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(File.Exists(filePathOneDriveLaptop))
            {
                Console.WriteLine("Wykryta maszyna: LAPTOP");
                FileInfo fi = new FileInfo(filePathOneDriveLaptop);
                long fileSize = fi.Length;

                if (fileSize <= 0)
                {
                    Console.WriteLine("Plik istnieje na dysku OneDrive, ale jest pusty.");

                    if (File.Exists(filePathLocal))
                    {
                        Console.WriteLine("Baza danych(plik TXT) istnieje lokalnie. Pobieram jego dane.");
                        StreamReader srLocal = new StreamReader(filePathLocal);
                        Dictionary<string, string> questionsLocal = new Dictionary<string, string>();
                        string lineLocal = "";
                        string[] lineLocalSplited;

                        do
                        {
                            lineLocal = srLocal.ReadLine();
                            lineLocalSplited = lineLocal.Split(';');
                            questionsLocal.Add(lineLocalSplited[0], lineLocalSplited[1]);
                        }
                        while (!srLocal.EndOfStream);

                        srLocal.Close();
                        srLocal.Dispose();

                        Console.WriteLine("Aktualizuję dysk OneDrive. Zapisywanie danych z pliku lokalnego.");

                        StreamWriter sw = new StreamWriter(filePathOneDriveLaptop, false);

                        foreach (KeyValuePair<string, string> question in questionsLocal)
                        {
                            sw.WriteLine(question.Key + ";" + question.Value);
                        }

                        sw.Close();
                        sw.Dispose();

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Plik TXT na dysku OneDrive został zaktualizowany.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Nie można zaktualizować dysku OneDrive. Brak bazy danych(pliku TXT) lokalnie.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Plik na dysku OneDrive ma pojemność {fileSize} bajtów.");
                    Console.ForegroundColor = ConsoleColor.White;

                    StreamReader sr = new StreamReader(filePathOneDriveLaptop);
                    Dictionary<string, string> questionsLaptop = new Dictionary<string, string>();
                    string line = "";
                    string[] lineSplitted;
                    do
                    {
                        line = sr.ReadLine();
                        lineSplitted = line.Split(';');
                        questionsLaptop.Add(lineSplitted[0], lineSplitted[1]);
                    }
                    while (!sr.EndOfStream);

                    sr.Close();
                    sr.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Plik został pobrany z dysku OneDrive.");
                    Console.WriteLine("Sprawdzam czy plik TXT istnieje lokalnie.");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("ENTER-->kontynuuj");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    Console.Clear();

                    if (File.Exists(filePathLocal))
                    {
                        Console.WriteLine("Baza danych(plik TXT) istnieje lokalnie. Sprawdzam aktualność danych.");
                        StreamReader srLocal = new StreamReader(filePathLocal);
                        Dictionary<string, string> questionsLocal = new Dictionary<string, string>();
                        string lineLocal = "";
                        string[] lineLocalSplited;

                        do
                        {
                            lineLocal = srLocal.ReadLine();
                            lineLocalSplited = lineLocal.Split(';');
                            questionsLocal.Add(lineLocalSplited[0], lineLocalSplited[1]);
                        }
                        while (!srLocal.EndOfStream);

                        srLocal.Close();
                        srLocal.Dispose();

                        int questionsOneDriveCount = questionsLaptop.Count();
                        int questionsLocalCount = questionsLocal.Count();

                        if (questionsOneDriveCount > questionsLocalCount)
                        {
                            Console.WriteLine($"Ilość pytań w lokalnej bazie({questionsLocalCount}) jest mniejsza niż na dysku OneDrive({questionsOneDriveCount})");
                            Console.WriteLine("Aktualizuję lokalny pik TXT.");

                            StreamWriter sw = new StreamWriter(filePathLocal, false);

                            foreach (KeyValuePair<string, string> question in questionsLaptop)
                            {
                                sw.WriteLine(question.Key + ";" + question.Value);
                            }

                            sw.Close();
                            sw.Dispose();

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Plik TXT lokalny został zaktualizowany.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (questionsOneDriveCount < questionsLocalCount)
                        {
                            Console.WriteLine($"Ilość pytań w lokalnej bazie({questionsLocalCount}) jest większa niż na dysku OneDrive({questionsOneDriveCount})");
                            Console.WriteLine("Aktualizuję dysk OneDrive.");

                            StreamWriter sw = new StreamWriter(filePathOneDriveLaptop, false);

                            foreach (KeyValuePair<string, string> question in questionsLocal)
                            {
                                sw.WriteLine(question.Key + ";" + question.Value);
                            }

                            sw.Close();
                            sw.Dispose();

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Plik TXT na dysku OneDrive został zaktualizowany.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine($"Ilość pytań w lokalnej bazie({questionsLocalCount}) jest równa ilośći pytań na dysku OneDrive({questionsOneDriveCount})");

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("Wszystko jest aktualne.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Baza danych(plik TXT) nie istnieje lokalnie.");
                        Console.WriteLine("Tworzę lokalną bazę danych na podstawie pliku z dysku OneDrive.");

                        StreamWriter sw = new StreamWriter(filePathLocal, false);

                        foreach (KeyValuePair<string, string> question in questionsLaptop)
                        {
                            sw.WriteLine(question.Key + ";" + question.Value);
                        }

                        sw.Close();
                        sw.Dispose();

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Plik TXT lokalny został utworzony.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
            }
            else if(!File.Exists(filePathOneDriveLaptop) && Directory.Exists(machineLaptop))
            {
                Console.WriteLine("Wykryta maszyna: LAPTOP");
                
                Console.ForegroundColor= ConsoleColor.Magenta;
                Console.WriteLine("Plik TXT na dysku OneDrive nie istnieje.");
                Console.WriteLine("Sprawdzam czy istnieje lokalnie.");
                Console.ForegroundColor = ConsoleColor.White;

                if(File.Exists(filePathLocal)) 
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Znaleziono lokalny plik TXT.");
                    Console.WriteLine("Pobieram dane z lokalnego pliku.");
                    Console.ForegroundColor = ConsoleColor.White;

                    Dictionary<string, string> questionsLocal = new Dictionary<string, string>();
                    StreamReader sr = new StreamReader(filePathLocal);
                    string lineLocal = "";
                    string[] lineLocalSplited;

                    do
                    {
                        lineLocal = sr.ReadLine();
                        lineLocalSplited = lineLocal.Split(';');
                        questionsLocal.Add(lineLocalSplited[0], lineLocalSplited[1]);
                    }
                    while(!sr.EndOfStream);

                    sr.Close();
                    sr.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Zapisuję kopię danych na dysku OneDrive.");
                    Console.ForegroundColor = ConsoleColor.White;

                    StreamWriter sw = new StreamWriter(filePathOneDriveLaptop, false);

                    foreach(KeyValuePair<string, string> question in questionsLocal)
                    {
                        sw.WriteLine(question.Key + ";" + question.Value);
                    }

                    sw.Close();
                    sw.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Zapis kopii pliku lokalnego na dysku OneDrive zakończony.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Nie można utworzyć kopii na dysku OneDrive. Lokalny plik TXT nie istnieje.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                Console.WriteLine("Nie można uzyskac dostępu do dysku OnDrive.");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Losowanie pytań ***DLA DODAWANIA NOWEJ KATEGORII EDYCJA TUTAJ**
        static void QuestionsLoterry(Dictionary<string, string> questionsLoterry, List<string> categories)
        {
            string path = "questions.txt";

            if (!File.Exists(path))
            {
                Console.WriteLine("Error - nie można wczytać pytań. Baza danych (plik TXT) nie istnieje.");
            }
            else
            {
                int input = RunMenu(categories);

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Wczytuję pytania z pliku z kategorii: {input}: {categories[input]}");
                Console.ForegroundColor = ConsoleColor.White;

                StreamReader sr = new StreamReader("questions.txt");

                string line = "";
                string[] lineSplited;
                int countLines = 1;

                do
                {
                    line = sr.ReadLine();
                    lineSplited = line.Split(';');

                    if(input == 0)
                    {
                        questionsLoterry.Add(lineSplited[0], lineSplited[1]);
                    }
                    else if(input == 1)
                    {
                        if (lineSplited[0].StartsWith("INŻ:"))
                        {
                            questionsLoterry.Add(lineSplited[0], lineSplited[1]);
                        }
                    }
                    else if(input == 2)
                    {
                        if(lineSplited[0].StartsWith("INŻ-2:"))
                        {
                            questionsLoterry.Add(lineSplited[0], lineSplited[1]);
                        }
                    }
                    else if (input == 3)
                    {
                        if (lineSplited[0].StartsWith("INŻ-3:"))
                        {
                            questionsLoterry.Add(lineSplited[0], lineSplited[1]);
                        }
                    }
                    else if (input == 4)
                    {
                        if (lineSplited[0].StartsWith("INŻ-4:"))
                        {
                            questionsLoterry.Add(lineSplited[0], lineSplited[1]);
                        }
                    }

                    countLines++;
                }
                while (!sr.EndOfStream);

                sr.Close();
                sr.Dispose();

                Console.WriteLine($"Ilość pytań do losowania: {questionsLoterry.Count} z kategorii: {categories[input]}");

                Console.WriteLine("ENTER --> rozpocznij losowanie");
                Console.ReadKey();

                do
                {
                    Random random = new Random();
                    int questionPicked = random.Next(0, questionsLoterry.Count);
                    int numerator = 0;

                    foreach (KeyValuePair<string, string> question in questionsLoterry)
                    {
                        if (questionPicked == numerator)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine(question.Key);
                            Console.ForegroundColor = ConsoleColor.White;

                            Console.Write("Odpowiedź: ");
                            string answer = Console.ReadLine();
                            Console.Clear();

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Twoja odpowiedź:");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(answer);

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Odpowiedź z bazy pytań:");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(question.Value);
                            Console.WriteLine("------------------------------------------------------------");

                            

                            Console.WriteLine("ENTER --> następne pytanie.");
                            Console.ReadKey();

                            if(questionsLoterry.Remove(question.Key))
                            {
                                Console.WriteLine("Usuwanie pytanie z listy. OK");
                                Console.WriteLine("Pozostało pytań: " + questionsLoterry.Count);
                            }
                        }
                        numerator++;
                    }
                }
                while (questionsLoterry.Count != 0);
                Console.WriteLine("Odpowiedziano na wszystkie pytania.");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Ustawienia konsoli
        static void ConsoleSettings()
        {
            var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
            //Console.Title = "FileCrudServices" + $" [v: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}]";
            Console.Title = "FileCrudServices" + $" [v: {version.Major}.{version.Minor}.{version.Build}]";
        }

        // Menu
        static int RunMenu(List<string> menu)
        {
            // 1.--> Tworzenie zmiennej przechowującej wciśnięty klawisz
            ConsoleKeyInfo keyInfo;
            int currentChoice = 0;
            int menuItemChoosen = -1;
            bool isChoosen = false;

            do
            {
                for (int i = 0; i < menu.Count; i++) // wyświetl elementy menu
                {
                    // Ustaw kolory dla BG i czcionki elementów menu
                    if (currentChoice == i)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(menu[i] + " <--");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(menu[i]);
                    }
                }

                // 2.--> Użycie zmiennej przechowującej wciśnięty klawisz do odczytu wejścia
                keyInfo = Console.ReadKey();

                switch (keyInfo.Key) // obsługa inputa
                {
                    // 3.--> Użycie zmiennej przechowującej wciśnięty klawisz do podjęcia akcji (zmiana wyboru elementu menu)
                    case ConsoleKey.DownArrow:
                        if (currentChoice >= menu.Count - 1) currentChoice = 0;
                        else currentChoice++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (currentChoice <= 0) currentChoice = menu.Count - 1;
                        else currentChoice--;
                        break;
                    case ConsoleKey.Enter:
                        menuItemChoosen = currentChoice;
                        isChoosen = true;
                        break;
                }
                Console.Clear();
            }
            while (isChoosen == false);

            return menuItemChoosen;
        }

        // Stwórz pytanie ***DLA DODAWANIA NOWEJ KATEGORII EDYCJA TUTAJ**
        static string CreateQuestion(List<string> category)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Wybierz dla jakiej kategorii stworzyć pytanie. ENTER-->kontynuuj");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadKey();
            Console.Clear();

            int categoryChoice = RunMenu(category);

            string question = "";

            do
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"Podaj treść pytania dla kategorii {category[categoryChoice]}: ");
                Console.ForegroundColor = ConsoleColor.White;

                if (categoryChoice == 0)
                {
                    question = Console.ReadLine();
                }
                else if(categoryChoice == 1)
                {
                    question = "INŻ: " + Console.ReadLine();
                }
                else if(categoryChoice == 2)
                {
                    question = "INŻ-2: " + Console.ReadLine();
                }
                else if (categoryChoice == 3)
                {
                    question = "INŻ-3: " + Console.ReadLine();
                }
                else if (categoryChoice == 4)
                {
                    question = "INŻ-4: " + Console.ReadLine();
                }

            }
            while (question == null || question == "");

            return question;
        }

        // Stwórz odpowiedź
        static string CreateAnswer(string question)
        {
            string answer = "";
            do
            {
                Console.Write("Podaj odpowiedź dla pytania: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(question);
                Console.ForegroundColor = ConsoleColor.White;

                answer = Console.ReadLine();
            }
            while (answer == null || answer == "");

            return answer;
        }

        // Dodaj pytanie i odpowiedź do słownika
        static void AddToDictionary(Dictionary<string, string> questionsTemporary, string question, string answer)
        {
            if(question != null && answer != null)
            {
                questionsTemporary.Add(question, answer);
                Console.WriteLine("Pomyślnie dodano pytanie:");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(question);
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Oraz odpowiedź:");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(answer);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine("Nie można dodać nowego pytania. Pusta wartość");
            }
            

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Dopisz na końcu pliku TXT obecne elementy kolekcji
        static void AppendToTxtFile(Dictionary<string, string> questionsTemporary)
        {
           if(questionsTemporary == null || questionsTemporary.Count == 0)
           {
               Console.WriteLine("Brak nowych pytań w kolekcji");
           }
           else
           {
                StreamWriter sw = new StreamWriter("questions.txt", true);
                string line = "";

                foreach (KeyValuePair<string, string> question in questionsTemporary)
                {
                    line = question.Key + ";" + question.Value;
                    sw.WriteLine(line);
                }


                sw.Close();
                sw.Dispose();

                questionsTemporary.Clear();

                Console.WriteLine("Pomyślnie dopisano do pliku nowe pytania.");
           }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Wyświetl obecny stan kolekcji
        static void DisplayList(Dictionary<string, string> questionsTemporary)
        {
            if(questionsTemporary.Count== 0)
            {
                Console.WriteLine("Obecnie kolekcja jest pusta.");
            }
            else
            {
                foreach (KeyValuePair<string, string> question in questionsTemporary)
                {
                    Console.WriteLine(question.Key);
                    Console.WriteLine();
                    Console.WriteLine(question.Value);
                    Console.WriteLine("----------------------------------------------------------------------------");
                }
            }
           
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Wyświetl obecną zawartość pliku TXT
        static void DisplayTxtFile()
        {
            string path = "questions.txt";

            if (!File.Exists(path))
            {
                Console.WriteLine("Baza danych (plik TXT) nie istnieje.");
            }
            else
            {
                StreamReader sr = new StreamReader("questions.txt");

                string line = "";
                string[] lineSplited;
                int countLines = 1;

                do
                {
                    line = sr.ReadLine();
                    lineSplited = line.Split(';');
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(lineSplited[0]);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine(lineSplited[1]);
                    Console.WriteLine("------------------------------------------------------------------------------------------");
                    countLines++;
                } 
                while (!sr.EndOfStream);

                sr.Close();
                sr.Dispose();
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Edytuj pytanie lub odpowiedź w pliku TXT
        static void EditTxtFile()
        {
            Dictionary<string, string> questionsToEdit = new Dictionary<string, string>();
            Dictionary<string, string> questionsReadyToSave = new Dictionary<string, string>();
            questionsToEdit.Clear();
            questionsReadyToSave.Clear();

            string file = "questions.txt";

            if(File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);

                do
                {
                    string line = sr.ReadLine();
                    string[] lineSplited = line.Split(';');
                    questionsToEdit.Add(lineSplited[0], lineSplited[1]);
                }
                while (!sr.EndOfStream);

                sr.Close();
                sr.Dispose();

                int questionNumber = 0;

                foreach (KeyValuePair<string, string> question in questionsToEdit)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(questionNumber + ": " + question.Key);
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine(question.Value);
                    Console.WriteLine("--------------------------------------------------------");

                    questionNumber++;
                }

                bool edited = false;
                do
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    int howManyQuestionsInDict = questionsToEdit.Count - 1;
                    Console.Write($"Numer klucza do edycji(0-{howManyQuestionsInDict}):");
                    Console.ForegroundColor = ConsoleColor.White;

                    string input = Console.ReadLine();

                    int inputParsed;
                    if (int.TryParse(input, out inputParsed))
                    {
                        if(inputParsed < 0 || inputParsed > questionsToEdit.Count-1)
                        {
                            Console.WriteLine($"Zestaw o numerze {inputParsed} nie istnieje.");
                        }
                        else
                        {
                            Console.WriteLine($"Chcesz edytować pytanie(q) czy odpowiedź(a) dla zestawu {inputParsed}? Anuluj(end)");
                            string input2 = Console.ReadLine();

                            if (input2 == "q")
                            {
                                string newQuestion = "";
                                string currentQuestion = "";
                                string currentAnswer = "";
                                int iterator = 0;

                                foreach (KeyValuePair<string, string> question in questionsToEdit)
                                {
                                    if (inputParsed == iterator)
                                    {
                                        Console.Clear();
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine(iterator + ": " + question.Key);
                                        Console.ForegroundColor = ConsoleColor.White;

                                        Console.WriteLine(question.Value);
                                        Console.WriteLine("--------------------------------------------------------");

                                        currentQuestion = question.Key;
                                        currentAnswer = question.Value;
                                    }
                                    iterator++;
                                }

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Podaj nową wartość pytania:");
                                Console.ForegroundColor = ConsoleColor.White;

                                newQuestion = Console.ReadLine();

                                questionsToEdit.Remove(currentQuestion);

                                questionsToEdit.Add(newQuestion, currentAnswer);

                                Console.Clear();

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Nowa wartość pytania to:");
                                Console.ForegroundColor = ConsoleColor.White;

                                Console.WriteLine(newQuestion);

                                foreach (KeyValuePair<string, string> question in questionsToEdit)
                                {
                                    questionsReadyToSave.Add(question.Key, question.Value);
                                }

                                StreamWriter sw = new StreamWriter(file, false);

                                foreach (KeyValuePair<string, string> question in questionsReadyToSave)
                                {
                                    sw.WriteLine(question.Key + ";" + question.Value);
                                }

                                sw.Close();
                                sw.Dispose();

                                Console.WriteLine("Dane zostały zapisane. ENTER--> kontynuuj");
                                edited = true;
                                Console.ReadKey();
                            }
                            else if (input2 == "a")
                            {
                                string newAnswer = "";
                                string currentQuestion = "";
                                string currentAnswer = "";
                                int iterator = 0;

                                foreach (KeyValuePair<string, string> question in questionsToEdit)
                                {
                                    if (inputParsed == iterator)
                                    {
                                        Console.Clear();
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine(iterator + ": " + question.Key);
                                        Console.ForegroundColor = ConsoleColor.White;

                                        Console.WriteLine(question.Value);
                                        Console.WriteLine("--------------------------------------------------------");

                                        currentQuestion = question.Key;
                                        currentAnswer = question.Value;
                                    }
                                    iterator++;
                                }

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Podaj nową wartość odpowiedzi:");
                                Console.ForegroundColor = ConsoleColor.White;

                                newAnswer = Console.ReadLine();

                                questionsToEdit.Remove(currentQuestion);

                                questionsToEdit.Add(currentQuestion, newAnswer);

                                Console.Clear();

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Nowa wartość odpowiedzi to:");
                                Console.ForegroundColor = ConsoleColor.White;

                                Console.WriteLine(newAnswer);

                                foreach (KeyValuePair<string, string> question in questionsToEdit)
                                {
                                    questionsReadyToSave.Add(question.Key, question.Value);
                                }

                                StreamWriter sw = new StreamWriter(file, false);

                                foreach (KeyValuePair<string, string> question in questionsReadyToSave)
                                {
                                    sw.WriteLine(question.Key + ";" + question.Value);
                                }

                                sw.Close();
                                sw.Dispose();

                                Console.WriteLine("Dane zostały zapisane. ENTER--> kontynuuj");
                                edited = true;
                                Console.ReadKey();
                            }
                            else if (input2 == "end")
                            {
                                Console.Clear();
                                return;
                            }
                            else Console.WriteLine("Błędny wybór.");
                        }
                    }
                    else Console.WriteLine("Błędna wartość.");
                }
                while (edited == false);
            }
            else
            {
                Console.WriteLine("Baza danych (plik TXT) nie istnieje.");
            }
            

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Usuń plik TXT
        static void DeleteTxtFile()
        {
            if(File.Exists("questions.txt"))
            {
                Console.WriteLine("Na pewno usunąć bazę danych (plik TXT)? y/n");
                string confirm = Console.ReadLine();
                if (confirm == "y")
                {
                    File.Delete("questions.txt");
                    Console.WriteLine("Baza danych (plik TXT) została pomyślnie usunięta.");
                }
                else Console.WriteLine("Anulowanie operacji usunięcia bazy danych.");
            }
            else
            {
                Console.WriteLine("Baza danych (plik TXT) nie istnieje");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Usuń pojedynczy rekord z pliku TXT
        static void DeleteFromTxtById()
        {
            if(File.Exists("questions.txt"))
            {
                // Pobierz pytania z pliku txt i dodaj do słownika
                Dictionary<string, string> questions = new Dictionary<string, string>();
                StreamReader sr = new StreamReader("questions.txt");
                string line = "";
                string[] lineSplitted;

                do
                {
                    line = sr.ReadLine();
                    lineSplitted = line.Split(';');
                    questions.Add(lineSplitted[0], lineSplitted[1]);
                } 
                while(!sr.EndOfStream);

                sr.Close();
                sr.Dispose();

                // Wyświetl słownik
                int id = 0;
                foreach (KeyValuePair<string, string> question in questions)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(id + ": " + question.Key);
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine(question.Value);
                    Console.WriteLine("------------------------------------------------------------------------------");
                    id++;
                }


                
                bool isParsed = false;
                int inputParsed;
                do
                {
                    // Pobierz od usera numer id pytania do usuniecia i parsuj go na int
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("Podaj numer id pytania do usunięcia: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    string input = Console.ReadLine();
                    
                    if (int.TryParse(input, out inputParsed))
                    {
                        inputParsed = int.Parse(input);

                        if (inputParsed < 0 || inputParsed > questions.Count-1)
                        {
                            Console.WriteLine($"ID o numerze {inputParsed} nie istnieje.");
                        }
                        else
                        {
                            inputParsed = int.Parse(input);
                            isParsed = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Błędna wartość.");
                    }
                }
                while (isParsed == false);


                // pobierz ze słownika rekord o podanym ID
                id = 0;
                string questionToDelete = "";

                foreach (KeyValuePair<string, string> question in questions)
                {
                    if (inputParsed == id)
                    {
                        questionToDelete = question.Key;
                    }
                    id++;
                }

                // usuń ze słownika rekord
                bool deleted = questions.Remove(questionToDelete);


                // Utwórz nowy plik TXT z zaktualizowanymi danymi
                if (deleted == false)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Nie udało się usunać rekordu:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(questionToDelete);
                }
                else
                {
                    StreamWriter sw = new StreamWriter("questions.txt");
                    string lineCombined = "";

                    foreach(KeyValuePair<string, string> question in questions)
                    {
                        lineCombined = question.Key + ";" + question.Value;
                        sw.WriteLine(lineCombined);
                    }

                    sw.Close();
                    sw.Dispose();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Rekord pomyślnie usunięty. Baza danych zaktualizowana.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                Console.WriteLine("Baza danych (plik TXT) nie istnieje.");
            }



            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ENTER-->powrót do menu");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }
    }
}