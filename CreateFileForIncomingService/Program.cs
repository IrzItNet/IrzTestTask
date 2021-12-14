using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace CreateFileForIncomingService
{
    class Program
    {
        private static Dictionary<SubjectArea, List<Document>> subjectAreaDocumentComaration =
            new Dictionary<SubjectArea, List<Document>>()
            {
                [SubjectArea.D01] = new List<Document>()
                {
                    Document.ПриобретениеТоваровУслуг,
                    Document.РеализацияТоваровУслуг,
                    Document.ПриобретениеУслугПрочихАктивов,
                },

                [SubjectArea.Tmc] = new List<Document>()
                {
                    Document.СостоянияСомнительностиСерий,
                    Document.ПеремещениеТоваровМеждуОрганизациямиИРЗ,
                    Document.СерииНоменклатуры,
                },

                [SubjectArea.Bank] = new List<Document>()
                {
                    Document.СБДСРасчетыСКонтрагентами,
                    Document.СчетФактураПолученный,
                    Document.СчетФактураВыданныйАванс,
                },
            };

        static void Main()
        {
            var path = ConfigurationManager.AppSettings["PathForFilesIncomingService"];
            if(Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            IExampleDocumentForCheckGeter exampleDocumentForCheckGeter = new FakeExampleDocumentForCheckGeter();
            
            var subjectAreaSet = new HashSet<SubjectArea> { SubjectArea.D01, SubjectArea.Tmc, SubjectArea.Bank };
            foreach (var subjectArea in subjectAreaSet)
            {
                var subjectAreaPath = Path.Combine(path, subjectArea.ToString());
                Directory.CreateDirectory(subjectAreaPath);

                var documents = subjectAreaDocumentComaration[subjectArea];

                foreach (var document in documents)
                {
                    var documentPath = Path.Combine(subjectAreaPath, document.ToString());
                    Directory.CreateDirectory(documentPath);

                    var packages = exampleDocumentForCheckGeter.Get(document, 10);
                    foreach (var package in packages)
                    {
                        var filePath = Path.Combine(documentPath, package.Id + ".xml");
                        File.WriteAllText(filePath, package.Text);
                    }
                }
            }
        }
    }
}
