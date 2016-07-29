# ETL-Extractor
Extractor of ETL
Use following example:
    var result = new List<string>();
    var marker = FileMarkerManager.CreateNew(123);
    FileTaker taker = new FileTaker(@"(\r\n|\n)",
        new FileGetterFactory().GetFileGetterByName(ResourceType.FTPServer, new string[] { user, pass }),
        new FileFilterFactory().GetFileFilterByName(IncreamentationType.InFileIncreamentation),
        marker,
        new DateTime(2016,7,20));

    var errors = taker.ExtractOnce(r => { result.AddRange(r); return true; }, destination, SearchOption.AllDirectories, ".fileExtension");
    FileMarkerManager.Save(marker);
