namespace Mono.TextTemplating.Utility.EntityFramework
{
    /// <summary>
    /// Responsible for encapsulating the constants defined in Metadata
    /// </summary>
    public static class MetadataConstants
    {
        public const string EDMX_NAMESPACE_V1 = "http://schemas.microsoft.com/ado/2007/06/edmx";
        public const string EDMX_NAMESPACE_V2 = "http://schemas.microsoft.com/ado/2008/10/edmx";


        public const string CSDL_EXTENSION = ".csdl";
        public const string CSDL_NAMESPACE_V1 = "http://schemas.microsoft.com/ado/2006/04/edm";
        public const string CSDL_NAMESPACE_V2 = "http://schemas.microsoft.com/ado/2008/09/edm";
        public const string CSDL_EDMX_SECTION_NAME = "ConceptualModels";
        public const string CSDL_ROOT_ELEMENT_NAME = "Schema";
        public const string EDM_ANNOTATION_09_02 = "http://schemas.microsoft.com/ado/2009/02/edm/annotation";

        public const string SSDL_EXTENSION = ".ssdl";
        public const string SSDL_NAMESPACE_V1 = "http://schemas.microsoft.com/ado/2006/04/edm/ssdl";
        public const string SSDL_NAMESPACE_V2 = "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
        public const string SSDL_EDMX_SECTION_NAME = "StorageModels";
        public const string SSDL_ROOT_ELEMENT_NAME = "Schema";

        public const string MSL_EXTENSION = ".msl";
        public const string MSL_NAMESPACE_V1 = "urn:schemas-microsoft-com:windows:storage:mapping:CS";
        public const string MSL_NAMESPACE_V2 = "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
        public const string MSL_EDMX_SECTION_NAME = "Mappings";
        public const string MSL_ROOT_ELEMENT_NAME = "Mapping";
    }
}