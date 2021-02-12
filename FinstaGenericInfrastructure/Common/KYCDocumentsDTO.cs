using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Common
{
    public class KYCDocumentsDTO : CommonDTO
    {
        public Int64 pDocumentId { get; set; }
        public Int64 pDocumentGroupId { get; set; }
        public string pDocumentGroup { get; set; }
        public string pDocumentName { get; set; }
        public string pDocStorePath { get; set; }
        public string pDocFileType { get; set; }
        public string pDocReferenceno { get; set; }       
        public Boolean pDocIsDownloadable { get; set; }
        public string pDocumentReferenceMonth { get; set; }
        public string pDocumentReferenceYear { get; set; }
        public string pFilename { get; set; }
    }

    public class RelationShipDTO 
    {
        public Int64 pRelationShipId { get; set; }
        public string pRelationShipName { get; set; }
    }
}
