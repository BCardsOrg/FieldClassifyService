using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.DataModel
{
    [Flags]
    public enum EntityRelation
    {
        /// <summary>
        /// The undefined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The owner
        /// </summary>
        Owner = 1,
        /// <summary>
        /// The link
        /// </summary>
        Link = 2,
        /// <summary>
        /// All
        /// </summary>
        All = Owner | Link
    };

    /// <summary>
    /// 
    /// </summary>
    public enum EntityTreeChange
    {
        /// <summary>
        /// The undefined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Add a child
        /// </summary>
        ChildAdd,
        /// <summary>
        /// Create a child
        /// </summary>
        ChildCreate,
        /// <summary>
        /// Remove a child 
        /// </summary>
        ChildRemove,
        /// <summary>
        /// Rename
        /// </summary>
        Rename
    }

    /// <summary>
    /// 
    /// </summary>
    public enum EntityTreeEventTiming
    {
        /// <summary>
        /// The pre change
        /// </summary>
        PreChange,
        /// <summary>
        /// The post change
        /// </summary>
        PostChange
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags()]
    public enum EntityCloneSpec
    {
        /// <summary>
        /// The data
        /// </summary>
        Data = 1,
        /// <summary>
        /// The attachments
        /// </summary>
        Attachments = 2,
        /// <summary>
        /// The owned children
        /// </summary>
        OwnedChildren = 4,
        /// <summary>
        /// The linked children
        /// </summary>
        LinkedChildren = 8,
        /// <summary>
        /// All children
        /// </summary>
        AllChildren = OwnedChildren | LinkedChildren,
        /// <summary>
        /// All
        /// </summary>
        All = Data | Attachments | AllChildren
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_ACCESS_CATEGORY
    {
        /// <summary>
        /// Attachment
        /// </summary>
        TIS_ACCESS_ATTACHMENT = 1,
        /// <summary>
        /// Custom
        /// </summary>
        TIS_ACCESS_CUSTOM = 5,
        /// <summary>
        /// Field
        /// </summary>
        TIS_ACCESS_FIELD = 4,
        /// <summary>
        /// Flow
        /// </summary>
        TIS_ACCESS_FLOW = 2,
        /// <summary>
        /// Form
        /// </summary>
        TIS_ACCESS_FORM = 3,
        /// <summary>
        /// Station
        /// </summary>
        TIS_ACCESS_STATION = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum _TIS_ACCESS_RIGHTS_
    {
        /// <summary>
        /// The permission to configure the station
        /// </summary>
        TIS_ACCESS_CONFIGURE = 16,
        /// <summary>
        /// Custom access 
        /// </summary>
        TIS_ACCESS_CUSTOM1 = 1048576,
        /// <summary>
        /// Custom access 
        /// </summary>
        TIS_ACCESS_CUSTOM2 = 2097152,
        /// <summary>
        /// Custom access 
        /// </summary>
        TIS_ACCESS_CUSTOM3 = 4194304,
        /// <summary>
        /// Custom access 
        /// </summary>
        TIS_ACCESS_CUSTOM4 = 8388608,
        /// <summary>
        /// Custom access 
        /// </summary>
        TIS_ACCESS_CUSTOM5 = 16777216,
        /// <summary>
        /// The permission to debug 
        /// </summary>
        TIS_ACCESS_DEBUG = 32,
        /// <summary>
        /// The permission to delete
        /// </summary>
        TIS_ACCESS_DELETE = 8,
        /// <summary>
        /// The permission to run the station
        /// </summary>
        TIS_ACCESS_EXECUTE = 1,
        /// <summary>
        /// The access level 1
        /// </summary>
        TIS_ACCESS_LEVEL1 = 256,
        /// <summary>
        /// The access level 2
        /// </summary>
        TIS_ACCESS_LEVEL2 = 512,
        /// <summary>
        /// The access level 3
        /// </summary>
        TIS_ACCESS_LEVEL3 = 1024,
        /// <summary>
        /// The access level 4
        /// </summary>
        TIS_ACCESS_LEVEL4 = 2048,
        /// <summary>
        /// The access level 5
        /// </summary>
        TIS_ACCESS_LEVEL5 = 4096,
        /// <summary>
        /// The access level 6
        /// </summary>
        TIS_ACCESS_LEVEL6 = 8192,
        /// <summary>
        /// The access level 7
        /// </summary>
        TIS_ACCESS_LEVEL7 = 16384,
        /// <summary>
        /// The access level 8
        /// </summary>
        TIS_ACCESS_LEVEL8 = 32768,
        //TIS_ACCESS_POLICY1 = 256
        //TIS_ACCESS_POLICY2 = 512
        //TIS_ACCESS_POLICY3 = 1024
        //TIS_ACCESS_POLICY4 = 2048
        //TIS_ACCESS_POLICY5 = 4096
        //TIS_ACCESS_POLICY6 = 8192
        //TIS_ACCESS_POLICY7 = 16384
        //TIS_ACCESS_POLICY8 = 32768
        /// <summary>
        /// The permission to read
        /// </summary>
        TIS_ACCESS_READ = 2,
        /// <summary>
        /// The permission to write
        /// </summary>
        TIS_ACCESS_WRITE = 4
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_ALPHA_NUMERIC_POST_PROCESSING
    {
        /// <summary>
        /// The TI s_ DON t_ CHANG e_ ALPH a_ NUMERIC
        /// </summary>
        TIS_DONT_CHANGE_ALPHA_NUMERIC = 0,
        /// <summary>
        /// The TI s_ FORC e_ ALPHA
        /// </summary>
        TIS_FORCE_ALPHA = 2,
        /// <summary>
        /// The TI s_ FORC e_ NUMERIC
        /// </summary>
        TIS_FORCE_NUMERIC = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_ARRAY_ORIENTATION
    {
        /// <summary>
        /// The TI s_ HORIZONTA l_ ARRAY
        /// </summary>
        TIS_HORIZONTAL_ARRAY = 1,
        /// <summary>
        /// The TI s_ VERTICA l_ ARRAY
        /// </summary>
        TIS_VERTICAL_ARRAY = 0

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_AUTOMATIC_SHOW_LOOKUP
    {
        /// <summary>
        /// The TI s_ SHO w_ LOOKU p_ NEVER
        /// </summary>
        TIS_SHOW_LOOKUP_NEVER = 1,
        /// <summary>
        /// The TI s_ SHO w_ LOOKU p_ NORMAL
        /// </summary>
        TIS_SHOW_LOOKUP_NORMAL = 0,
        /// <summary>
        /// The TI s_ SHO w_ LOOKU p_ WHE n_ NO t_ VALID
        /// </summary>
        TIS_SHOW_LOOKUP_WHEN_NOT_VALID = 2

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_CASE_POST_PROCESSING
    {
        /// <summary>
        /// The TI s_ DON t_ CHANG e_ CASE
        /// </summary>
        TIS_DONT_CHANGE_CASE = 0,
        /// <summary>
        /// The TI s_ FORC e_ LOWERCASE
        /// </summary>
        TIS_FORCE_LOWERCASE = 2,
        /// <summary>
        /// The TI s_ FORC e_ UPPERCASE
        /// </summary>
        TIS_FORCE_UPPERCASE = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_CHAR_RECT_FILTER
    {
        /// <summary>
        /// The TI s_ NEVE r_ PAS s_ TH e_ CHA r_ RECT
        /// </summary>
        TIS_NEVER_PASS_THE_CHAR_RECT = 0,
        /// <summary>
        /// The TI s_ PAS s_ TH e_ CHA r_ REC t_ FO r_ AL l_ CHARS
        /// </summary>
        TIS_PASS_THE_CHAR_RECT_FOR_ALL_CHARS = 2,
        /// <summary>
        /// The TI s_ PAS s_ TH e_ CHA r_ REC t_ FO r_ BA d_ CHARS
        /// </summary>
        TIS_PASS_THE_CHAR_RECT_FOR_BAD_CHARS = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_COLLECTION_SOURCE
    {
        /// <summary>
        /// The TI s_ COLLECTIO n_ SOURC e_ ELECTRONIC
        /// </summary>
        TIS_COLLECTION_SOURCE_ELECTRONIC = 1,
        /// <summary>
        /// The TI s_ COLLECTIO n_ SOURC e_ IMAGE
        /// </summary>
        TIS_COLLECTION_SOURCE_IMAGE = 0

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_COLLECTION_TYPE
    {
        /// <summary>
        /// The TI s_ BATC h_ COLLECTION
        /// </summary>
        TIS_BATCH_COLLECTION = 0,
        /// <summary>
        /// Collection of forms
        /// </summary>
        TIS_FORM_COLLECTION = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_COPY_CONDITION
    {
        /// <summary>
        /// The TI s_ COP y_ ALWAYS
        /// </summary>
        TIS_COPY_ALWAYS = 2,
        /// <summary>
        /// The TI s_ COP y_ NEVER
        /// </summary>
        TIS_COPY_NEVER = 0,
        /// <summary>
        /// The TI s_ COP y_ O n_ RULE
        /// </summary>
        TIS_COPY_ON_RULE = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DATE_FORMAT
    {
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ DDMMYY
        /// </summary>
        TIS_DATE_FORMAT_DDMMYY = 1,
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ DDMMYYYY
        /// </summary>
        TIS_DATE_FORMAT_DDMMYYYY = 4,
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ MMDDYY
        /// </summary>
        TIS_DATE_FORMAT_MMDDYY = 2,
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ MMDDYYYY
        /// </summary>
        TIS_DATE_FORMAT_MMDDYYYY = 5,
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ NONE
        /// </summary>
        TIS_DATE_FORMAT_NONE = 0,
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ YYMMDD
        /// </summary>
        TIS_DATE_FORMAT_YYMMDD = 3,
        /// <summary>
        /// The TI s_ DAT e_ FORMA t_ YYYYMMDD
        /// </summary>
        TIS_DATE_FORMAT_YYYYMMDD = 6

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DISPLAY_CONDITION
    {
        /// <summary>
        /// The TI s_ DISPLA y_ ALWAYS
        /// </summary>
        TIS_DISPLAY_ALWAYS = 0,
        /// <summary>
        /// The TI s_ DISPLA y_ EMPTY
        /// </summary>
        TIS_DISPLAY_EMPTY = 2,
        /// <summary>
        /// The TI s_ DISPLA y_ NEVER
        /// </summary>
        TIS_DISPLAY_NEVER = 1,
        /// <summary>
        /// The TI s_ DISPLA y_ NO t_ EMPTY
        /// </summary>
        TIS_DISPLAY_NOT_EMPTY = 3,
        /// <summary>
        /// The TI s_ DISPLA y_ UNRECOGNIZED
        /// </summary>
        TIS_DISPLAY_UNRECOGNIZED = 4,
        /// <summary>
        /// The TI s_ DISPLA y_ ONCE
        /// </summary>
        TIS_DISPLAY_ONCE = 5,
        /// <summary>
        /// The TI s_ DISPLA y_ CUSTO m_ RULE
        /// </summary>
        TIS_DISPLAY_CUSTOM_RULE = 6

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DISPLAY_MODE
    {
        /// <summary>
        /// The TI s_ DISPLA y_ MOD e_ DEFAULT
        /// </summary>
        TIS_DISPLAY_MODE_DEFAULT = 2,
        /// <summary>
        /// The TI s_ GROU p_ MODE
        /// </summary>
        TIS_GROUP_MODE = 1,
        /// <summary>
        /// The TI s_ PAG e_ MODE
        /// </summary>
        TIS_PAGE_MODE = 0

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DISPLAY_ORDER
    {
        /// <summary>
        /// The TI s_ DISPLA y_ ORDE r_ B y_ FOR m_ TYPE
        /// </summary>
        TIS_DISPLAY_ORDER_BY_FORM_TYPE = 1,
        /// <summary>
        /// The TI s_ DISPLA y_ ORDE r_ B y_ GROU p_ TYPE
        /// </summary>
        TIS_DISPLAY_ORDER_BY_GROUP_TYPE = 2,
        /// <summary>
        /// The TI s_ DISPLA y_ ORDE r_ DEFAULT
        /// </summary>
        TIS_DISPLAY_ORDER_DEFAULT = 3,
        /// <summary>
        /// The TI s_ DISPLA y_ ORDE r_ ORIGINAL
        /// </summary>
        TIS_DISPLAY_ORDER_ORIGINAL = 0

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DOC_MERGE_ACTION
    {
        /// <summary>
        /// The TI s_ DO c_ MERG e_ ACTIO n_ US e_ FIRST
        /// </summary>
        TIS_DOC_MERGE_ACTION_USE_FIRST = 0,
        /// <summary>
        /// The TI s_ DO c_ MERG e_ ACTIO n_ US e_ FIRS t_ OCCURRENCE
        /// </summary>
        TIS_DOC_MERGE_ACTION_USE_FIRST_OCCURRENCE = 3,
        /// <summary>
        /// The TI s_ DO c_ MERG e_ ACTIO n_ US e_ LAST
        /// </summary>
        TIS_DOC_MERGE_ACTION_USE_LAST = 1,
        /// <summary>
        /// The TI s_ DO c_ MERG e_ ACTIO n_ US e_ LAS t_ OCCURRENCE
        /// </summary>
        TIS_DOC_MERGE_ACTION_USE_LAST_OCCURRENCE = 4,
        /// <summary>
        /// The TI s_ DO c_ MERG e_ ACTIO n_ US e_ MIDDLE
        /// </summary>
        TIS_DOC_MERGE_ACTION_USE_MIDDLE = 2

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DOC_MERGE_MODE
    {
        /// <summary>
        /// The TI s_ DO c_ MERG e_ DISALLOWED
        /// </summary>
        TIS_DOC_MERGE_DISALLOWED = 2,
        /// <summary>
        /// The TI s_ DO c_ MERG e_ MOD e_ AUTOMATIC
        /// </summary>
        TIS_DOC_MERGE_MODE_AUTOMATIC = 0,
        /// <summary>
        /// The TI s_ DO c_ MERG e_ MOD e_ MANUAL
        /// </summary>
        TIS_DOC_MERGE_MODE_MANUAL = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DOUBLE_TYPING_2ND_PASS_VIEW
    {
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN G_2 N d_ PAS s_ VIE w_ EMPTY
        /// </summary>
        TIS_DOUBLE_TYPING_2ND_PASS_VIEW_EMPTY = 0,
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN G_2 N d_ PAS s_ VIE w_ LAST
        /// </summary>
        TIS_DOUBLE_TYPING_2ND_PASS_VIEW_LAST = 2,
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN G_2 N d_ PAS s_ VIE w_ ORIGINAL
        /// </summary>
        TIS_DOUBLE_TYPING_2ND_PASS_VIEW_ORIGINAL = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DOUBLE_TYPING_CONFLICT_ACTION
    {
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN g_ CONFLIC t_ ACTIO n_ DISPLAY
        /// </summary>
        TIS_DOUBLE_TYPING_CONFLICT_ACTION_DISPLAY = 0,
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN g_ CONFLIC t_ ACTIO n_ EXCEPTION
        /// </summary>
        TIS_DOUBLE_TYPING_CONFLICT_ACTION_EXCEPTION = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_DOUBLE_TYPING_MODE
    {
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN g_ MOD e_ AUTOMATIC
        /// </summary>
        TIS_DOUBLE_TYPING_MODE_AUTOMATIC = 3,
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN g_ MOD e_ NONE
        /// </summary>
        TIS_DOUBLE_TYPING_MODE_NONE = 0,
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN g_ MOD e_ SECON d_ STATION
        /// </summary>
        TIS_DOUBLE_TYPING_MODE_SECOND_STATION = 2,
        /// <summary>
        /// The TI s_ DOUBL e_ TYPIN g_ MOD e_ SINGL e_ STATION
        /// </summary>
        TIS_DOUBLE_TYPING_MODE_SINGLE_STATION = 1

    }
    //Public Enum TIS_DYNAMIC_OBJECT
    //    TIS_DYNAMIC_COLLECTION = 0
    //    TIS_DYNAMIC_FIELD = 7
    //    TIS_DYNAMIC_FIELD_ARRAY = 6
    //    TIS_DYNAMIC_FIELD_GROUP = 4
    //    TIS_DYNAMIC_FIELD_TABLE = 5
    //    TIS_DYNAMIC_FOLDER = 1
    //    TIS_DYNAMIC_FORM = 2
    //    TIS_DYNAMIC_PAGE = 3

    //End Enum
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_ELECTRONIC_FORM_FORMAT
    {
        /// <summary>
        /// The TI s_ ELECTRONI c_ FOR m_ FORMA t_ ALL
        /// </summary>
        TIS_ELECTRONIC_FORM_FORMAT_ALL = 0,
        /// <summary>
        /// The TI s_ ELECTRONI c_ FOR m_ FORMA t_ EMAIL
        /// </summary>
        TIS_ELECTRONIC_FORM_FORMAT_EMAIL = 2,
        /// <summary>
        /// The TI s_ ELECTRONI c_ FOR m_ FORMA t_ XML
        /// </summary>
        TIS_ELECTRONIC_FORM_FORMAT_XML = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_EXPORT_DESTINATION
    {
        /// <summary>
        /// The TI s_ EXPOR t_ DESTINATIO n_ CUSTOM
        /// </summary>
        TIS_EXPORT_DESTINATION_CUSTOM = 2,
        /// <summary>
        /// The TI s_ EXPOR t_ DESTINATIO n_ FTP
        /// </summary>
        TIS_EXPORT_DESTINATION_FTP = 1,
        /// <summary>
        /// The TI s_ EXPOR t_ DESTINATIO n_ LAN
        /// </summary>
        TIS_EXPORT_DESTINATION_LAN = 0

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_EXPORT_FORMAT
    {
        /// <summary>
        /// The TI s_ EXPOR t_ COMM a_ DELIMITED
        /// </summary>
        TIS_EXPORT_COMMA_DELIMITED = 1,
        /// <summary>
        /// The TI s_ EXPOR t_ CUSTOM
        /// </summary>
        TIS_EXPORT_CUSTOM = 3,
        /// <summary>
        /// The TI s_ EXPOR t_ IN i_ FIL e_ STYLE
        /// </summary>
        TIS_EXPORT_INI_FILE_STYLE = 0,
        /// <summary>
        /// The TI s_ EXPOR t_ XM l_ FORMAT
        /// </summary>
        TIS_EXPORT_XML_FORMAT = 2

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_FIELD_MODE
    {
        /// <summary>
        /// The TI s_ FIEL d_ MOD e_ CHEC k_ GROUP
        /// </summary>
        TIS_FIELD_MODE_CHECK_GROUP = 2,
        /// <summary>
        /// The TI s_ FIEL d_ MOD e_ RADI o_ GROUP
        /// </summary>
        TIS_FIELD_MODE_RADIO_GROUP = 1,
        /// <summary>
        /// The TI s_ FIEL d_ MOD e_ STANDARD
        /// </summary>
        TIS_FIELD_MODE_STANDARD = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_FIELD_TYPE
    {
        /// <summary>
        /// The TI s_ ALPH a_ NUMERI c_ FIELD
        /// </summary>
        TIS_ALPHA_NUMERIC_FIELD = 6,
        /// <summary>
        /// The TI s_ BLO b_ FIELD
        /// </summary>
        TIS_BLOB_FIELD = 5,
        /// <summary>
        /// The TI s_ DAT e_ FIELD
        /// </summary>
        TIS_DATE_FIELD = 3,
        /// <summary>
        /// The TI s_ FLOA t_ FIELD
        /// </summary>
        TIS_FLOAT_FIELD = 2,
        /// <summary>
        /// The TI s_ INTEGE r_ FIELD
        /// </summary>
        TIS_INTEGER_FIELD = 1,
        /// <summary>
        /// The TI s_ TEX t_ FIELD
        /// </summary>
        TIS_TEXT_FIELD = 0,
        /// <summary>
        /// The TI s_ TIM e_ FIELD
        /// </summary>
        TIS_TIME_FIELD = 4

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_INPUT_FILE_FORMAT
    {
        /// <summary>
        /// The TI s_ INPU t_ FIL e_ FORMA t_ ALL
        /// </summary>
        TIS_INPUT_FILE_FORMAT_ALL = 0,
        /// <summary>
        /// The TI s_ INPU t_ FIL e_ FORMA t_ GIF
        /// </summary>
        TIS_INPUT_FILE_FORMAT_GIF = 3,
        /// <summary>
        /// The TI s_ INPU t_ FIL e_ FORMA t_ JPG
        /// </summary>
        TIS_INPUT_FILE_FORMAT_JPG = 2,
        /// <summary>
        /// The TI s_ INPU t_ FIL e_ FORMA t_ TIFF
        /// </summary>
        TIS_INPUT_FILE_FORMAT_TIFF = 1

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_LEARNING_DISPLAY_CONDITION
    {
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ ALWAYS
        /// </summary>
        TIS_LEARNING_DISPLAY_ALWAYS = 4,
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ I f_ CONFIDENC e_ LES S_100
        /// </summary>
        TIS_LEARNING_DISPLAY_IF_CONFIDENCE_LESS_100 = 3,
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ I f_ CONFIDENC e_ LES S_25
        /// </summary>
        TIS_LEARNING_DISPLAY_IF_CONFIDENCE_LESS_25 = 0,
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ I f_ CONFIDENC e_ LES S_50
        /// </summary>
        TIS_LEARNING_DISPLAY_IF_CONFIDENCE_LESS_50 = 1,
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ I f_ CONFIDENC e_ LES S_75
        /// </summary>
        TIS_LEARNING_DISPLAY_IF_CONFIDENCE_LESS_75 = 2,
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ NEVER
        /// </summary>
        TIS_LEARNING_DISPLAY_NEVER = 5,
        /// <summary>
        /// The TI s_ LEARNIN g_ DISPLA y_ UNRECOGNIZED
        /// </summary>
        TIS_LEARNING_DISPLAY_UNRECOGNIZED = 6

    }
    /// <summary>
    /// 
    /// </summary>
    public enum TIS_LEFT_RIGHT
    {
        /// <summary>
        /// The TI s_ LEFT
        /// </summary>
        TIS_LEFT = 0,
        /// <summary>
        /// The TI s_ RIGHT
        /// </summary>
        TIS_RIGHT = 1

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_LOOKUP_TABLE_TYPE
    {
        /// <summary>
        /// The TI s_ INTERNA l_ TABLE
        /// </summary>
        TIS_INTERNAL_TABLE = 0,
        /// <summary>
        /// The TI s_ OL e_ D b_ TABLE
        /// </summary>
        TIS_OLE_DB_TABLE = 3,
        /// <summary>
        /// The TI s_ ORACL e_ TABLE
        /// </summary>
        TIS_ORACLE_TABLE = 2,
        /// <summary>
        /// The TI s_ SQ l_ SERVE r_ TABLE
        /// </summary>
        TIS_SQL_SERVER_TABLE = 1

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_MARK_SIZE
    {
        /// <summary>
        /// The TI s_ MAR k_ LARGE
        /// </summary>
        TIS_MARK_LARGE = 2,
        /// <summary>
        /// The TI s_ MAR k_ MULTIPLE
        /// </summary>
        TIS_MARK_MULTIPLE = 3,
        /// <summary>
        /// The TI s_ MAR k_ NORMAL
        /// </summary>
        TIS_MARK_NORMAL = 1,
        /// <summary>
        /// The TI s_ MAR k_ SMALL
        /// </summary>
        TIS_MARK_SMALL = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_MODULE_ACCESS_FLAGS
    {
        /// <summary>
        /// The TI s_ MODUL e_ ACCES s_ PROPERT y_ ARRAY
        /// </summary>
        TIS_MODULE_ACCESS_PROPERTY_ARRAY = 3,
        /// <summary>
        /// The TI s_ MODUL e_ ACCES s_ PROPERT y_ BST r_ INDEX
        /// </summary>
        TIS_MODULE_ACCESS_PROPERTY_BSTR_INDEX = 4,
        /// <summary>
        /// The TI s_ MODUL e_ ACCES s_ PROPERT y_ NONE
        /// </summary>
        TIS_MODULE_ACCESS_PROPERTY_NONE = 0,
        /// <summary>
        /// The TI s_ MODUL e_ ACCES s_ PROPERT y_ READ
        /// </summary>
        TIS_MODULE_ACCESS_PROPERTY_READ = 1,
        /// <summary>
        /// The TI s_ MODUL e_ ACCES s_ PROPERT y_ WRITE
        /// </summary>
        TIS_MODULE_ACCESS_PROPERTY_WRITE = 2

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_OBJECT_FILTER
    {
        /// <summary>
        /// The TI s_ OBJEC t_ EXCLUD e_ ALL
        /// </summary>
        TIS_OBJECT_EXCLUDE_ALL = 0,
        /// <summary>
        /// The TI s_ OBJEC t_ FILTE r_ NORMAL
        /// </summary>
        TIS_OBJECT_FILTER_NORMAL = 1,
        /// <summary>
        /// The TI s_ OBJEC t_ INCLUD e_ SELECTED
        /// </summary>
        TIS_OBJECT_INCLUDE_SELECTED = 2

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_PAGE_ID_METHOD
    {
        /// <summary>
        /// The TI s_ PAG e_ IDENTIFIE d_ AUTOMATICALLY
        /// </summary>
        TIS_PAGE_IDENTIFIED_AUTOMATICALLY = 1,
        /// <summary>
        /// The TI s_ PAG e_ IDENTIFIE d_ MANUALLY
        /// </summary>
        TIS_PAGE_IDENTIFIED_MANUALLY = 2,
        /// <summary>
        /// The TI s_ PAG e_ NO t_ IDENTIFIED
        /// </summary>
        TIS_PAGE_NOT_IDENTIFIED = 0,
        /// <summary>
        /// The TI s_ PAG e_ RECEIVE d_ DEFAUL t_ ID
        /// </summary>
        TIS_PAGE_RECEIVED_DEFAULT_ID = 3

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_PAGE_MATCH_ALGORITHM
    {
        /// <summary>
        /// The FORMOUT
        /// </summary>
        FORMOUT,
        /// <summary>
        /// The SCRIPT
        /// </summary>
        SCRIPT,
        /// <summary>
        /// The TEMPLATE
        /// </summary>
        TEMPLATE,
        /// <summary>
        /// The DATAID
        /// </summary>
        DATAID,
        /// <summary>
        /// The SMART
        /// </summary>
        SMART
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_ROTATION
    {
        /// <summary>
        /// The RO T_0
        /// </summary>
        ROT_0,
        /// <summary>
        /// The RO T_90
        /// </summary>
        ROT_90,
        /// <summary>
        /// The RO T_180
        /// </summary>
        ROT_180,
        /// <summary>
        /// The RO T_270
        /// </summary>
        ROT_270
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_PARAMETER_FILTER
    {
        /// <summary>
        /// The TI s_ PARAMETE r_ EXCLUDE
        /// </summary>
        TIS_PARAMETER_EXCLUDE = 0,
        /// <summary>
        /// The TI s_ PARAMETE r_ FILTE r_ NO t_ SPECIFIED
        /// </summary>
        TIS_PARAMETER_FILTER_NOT_SPECIFIED = 1,
        /// <summary>
        /// The TI s_ PARAMETE r_ INCLUDE
        /// </summary>
        TIS_PARAMETER_INCLUDE = 2

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_PRINT_TYPE
    {
        /// <summary>
        /// The TI s_ BARCODE
        /// </summary>
        TIS_BARCODE = 3,
        /// <summary>
        /// The TI s_ HAN d_ PRINT
        /// </summary>
        TIS_HAND_PRINT = 1,
        /// <summary>
        /// The TI s_ MACHIN e_ PRINT
        /// </summary>
        TIS_MACHINE_PRINT = 2,
        /// <summary>
        /// The TI s_ UNKNOW n_ PRINT
        /// </summary>
        TIS_UNKNOWN_PRINT = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_PROCESS_MODE
    {
        /// <summary>
        /// The TI s_ PROCES s_ OC r_ ONLY
        /// </summary>
        TIS_PROCESS_OCR_ONLY = 2,
        /// <summary>
        /// The TI s_ PROCES s_ PAG e_ I d_ AN d_ OCR
        /// </summary>
        TIS_PROCESS_PAGE_ID_AND_OCR = 3,
        /// <summary>
        /// The TI s_ PROCES s_ PAG e_ I d_ I f_ REQUIRE d_ AN d_ OCR
        /// </summary>
        TIS_PROCESS_PAGE_ID_IF_REQUIRED_AND_OCR = 0,
        /// <summary>
        /// The TI s_ PROCES s_ PAG e_ I d_ ONLY
        /// </summary>
        TIS_PROCESS_PAGE_ID_ONLY = 1

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_PROCESS_TYPE
    {
        /// <summary>
        /// The TI s_ PROCES s_ COMPUTE r_ LIST
        /// </summary>
        TIS_PROCESS_COMPUTER_LIST = 1,
        /// <summary>
        /// The TI s_ PROCES s_ NORMAL
        /// </summary>
        TIS_PROCESS_NORMAL = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_RECT_SOURCE_TYPE
    {
        /// <summary>
        /// The TI s_ REC t_ SOURC e_ AUTOMATIC
        /// </summary>
        TIS_RECT_SOURCE_AUTOMATIC = 1,
        /// <summary>
        /// The TI s_ REC t_ SOURC e_ MANUA l_ DRAW
        /// </summary>
        TIS_RECT_SOURCE_MANUAL_DRAW = 2,
        /// <summary>
        /// The TI s_ REC t_ SOURC e_ MANUA l_ SELECTION
        /// </summary>
        TIS_RECT_SOURCE_MANUAL_SELECTION = 3,
        /// <summary>
        /// The TI s_ REC t_ SOURC e_ UNKNOWN
        /// </summary>
        TIS_RECT_SOURCE_UNKNOWN = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_REJECTED_COLLECTIONS_POLICY
    {
        /// <summary>
        /// The TI s_ REJECTE d_ COLLECTION s_ ALSO
        /// </summary>
        TIS_REJECTED_COLLECTIONS_ALSO = 2,
        /// <summary>
        /// The TI s_ REJECTE d_ COLLECTION s_ NONE
        /// </summary>
        TIS_REJECTED_COLLECTIONS_NONE = 0,
        /// <summary>
        /// The TI s_ REJECTE d_ COLLECTION s_ ONLY
        /// </summary>
        TIS_REJECTED_COLLECTIONS_ONLY = 1

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_RULE_INVALID_ACTION
    {
        /// <summary>
        /// The TI s_ RUL e_ INVALI d_ ATTAC h_ TA g_ AN d_ CONTINUE
        /// </summary>
        TIS_RULE_INVALID_ATTACH_TAG_AND_CONTINUE = 3,
        /// <summary>
        /// The TI s_ RUL e_ INVALI d_ SHO w_ MESSAG e_ AN d_ OPE n_ TAGS
        /// </summary>
        TIS_RULE_INVALID_SHOW_MESSAGE_AND_OPEN_TAGS = 2,
        /// <summary>
        /// The TI s_ RUL e_ INVALI d_ SHO w_ MESSAG e_ AN d_ QUER y_ USER
        /// </summary>
        TIS_RULE_INVALID_SHOW_MESSAGE_AND_QUERY_USER = 1,
        /// <summary>
        /// The TI s_ RUL e_ INVALI d_ SHO w_ MESSAG e_ AN d_ STOP
        /// </summary>
        TIS_RULE_INVALID_SHOW_MESSAGE_AND_STOP = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_RULE_MESSAGE_DISPLAY_TYPE
    {
        /// <summary>
        /// The TI s_ RUL e_ DIALO g_ BO x_ MSG
        /// </summary>
        TIS_RULE_DIALOG_BOX_MSG = 1,
        /// <summary>
        /// The TI s_ RUL e_ STATU s_ BA r_ MSG
        /// </summary>
        TIS_RULE_STATUS_BAR_MSG = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_SCANNER_COLOR_MODE
    {
        /// <summary>
        /// The TI s_ SCANNE r_ COLO r_ MOD e_ BLAC k_ WHITE
        /// </summary>
        TIS_SCANNER_COLOR_MODE_BLACK_WHITE = 0,
        /// <summary>
        /// The TI s_ SCANNE r_ COLO r_ MOD e_ COLOR
        /// </summary>
        TIS_SCANNER_COLOR_MODE_COLOR = 2,
        /// <summary>
        /// The TI s_ SCANNE r_ COLO r_ MOD e_ GRA y_ SCALE
        /// </summary>
        TIS_SCANNER_COLOR_MODE_GRAY_SCALE = 1

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_COMMON_SUBCATEGORIES
    {
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ IMPORTANT
        /// </summary>
        TIS_SUBCATEGORY_IMPORTANT = 1073741824,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ MAIN
        /// </summary>
        TIS_SUBCATEGORY_MAIN = 65536,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ MEMOR y_ MANAGEMENT
        /// </summary>
        TIS_SUBCATEGORY_MEMORY_MANAGEMENT = 131072,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ PERFORMANCE
        /// </summary>
        TIS_SUBCATEGORY_PERFORMANCE = 262144
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_LOG_CATEGORIES
    {
        /// <summary>
        /// The TI s_ CATEGOR y_ ARCHIV e_ APP
        /// </summary>
        TIS_CATEGORY_ARCHIVE_APP = 220,
        /// <summary>
        /// The TI s_ CATEGOR y_ AUT o_ GAT e_ APP
        /// </summary>
        TIS_CATEGORY_AUTO_GATE_APP = 201,
        /// <summary>
        /// The TI s_ CATEGOR y_ BACKU p_ APP
        /// </summary>
        TIS_CATEGORY_BACKUP_APP = 241,
        /// <summary>
        /// The TI s_ CATEGOR y_ BUIL d_ APP
        /// </summary>
        TIS_CATEGORY_BUILD_APP = 223,
        /// <summary>
        /// The TI s_ CATEGOR y_ CLIEN t_ LAUNCHE r_ APP
        /// </summary>
        TIS_CATEGORY_CLIENT_LAUNCHER_APP = 251,
        /// <summary>
        /// The TI s_ CATEGOR y_ CLIEN t_ SERVICE s_ MODULE
        /// </summary>
        TIS_CATEGORY_CLIENT_SERVICES_MODULE = 160,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLEC t_ APP
        /// </summary>
        TIS_CATEGORY_COLLECT_APP = 222,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLECTIO n_ CRITERIA
        /// </summary>
        TIS_CATEGORY_COLLECTION_CRITERIA = 120,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLECTIO n_ MANAGE r_ SERVER
        /// </summary>
        TIS_CATEGORY_COLLECTION_MANAGER_SERVER = 125,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLECTIO n_ PAT h_ HASH
        /// </summary>
        TIS_CATEGORY_COLLECTION_PATH_HASH = 103,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLECTIO n_ PRIORITY
        /// </summary>
        TIS_CATEGORY_COLLECTION_PRIORITY = 121,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLECTIO n_ REGISTRY
        /// </summary>
        TIS_CATEGORY_COLLECTION_REGISTRY = 112,
        /// <summary>
        /// The TI s_ CATEGOR y_ COLLECTIO n_ RUL e_ EVALUATOR
        /// </summary>
        TIS_CATEGORY_COLLECTION_RULE_EVALUATOR = 115,
        /// <summary>
        /// The TI s_ CATEGOR y_ COMPLETIO n_ APP
        /// </summary>
        TIS_CATEGORY_COMPLETION_APP = 213,
        /// <summary>
        /// The TI s_ CATEGOR y_ CONFIGURATO r_ APP
        /// </summary>
        TIS_CATEGORY_CONFIGURATOR_APP = 244,
        /// <summary>
        /// The TI s_ CATEGOR y_ CONTROLLE r_ APP
        /// </summary>
        TIS_CATEGORY_CONTROLLER_APP = 225,
        /// <summary>
        /// The TI s_ CATEGOR y_ DESIGNE r_ APP
        /// </summary>
        TIS_CATEGORY_DESIGNER_APP = 200,
        /// <summary>
        /// The TI s_ CATEGOR y_ DYNAMI c_ DAT a_ LAYE r_ CLIENT
        /// </summary>
        TIS_CATEGORY_DYNAMIC_DATA_LAYER_CLIENT = 166,
        /// <summary>
        /// The TI s_ CATEGOR y_ DYNAMI c_ DAT a_ LAYE r_ SERVER
        /// </summary>
        TIS_CATEGORY_DYNAMIC_DATA_LAYER_SERVER = 102,
        /// <summary>
        /// The TI s_ CATEGOR y_ EF i_ SCA n_ APP
        /// </summary>
        TIS_CATEGORY_EFI_SCAN_APP = 204,
        /// <summary>
        /// The TI s_ CATEGOR y_ EFOR m_ GAT e_ APP
        /// </summary>
        TIS_CATEGORY_EFORM_GATE_APP = 202,
        /// <summary>
        /// The TI s_ CATEGOR y_ EQUALIZER
        /// </summary>
        TIS_CATEGORY_EQUALIZER = 2,
        /// <summary>
        /// The TI s_ CATEGOR y_ EVEN t_ CONTROL
        /// </summary>
        TIS_CATEGORY_EVENT_CONTROL = 181,
        /// <summary>
        /// The TI s_ CATEGOR y_ EXCEPTIO n_ APP
        /// </summary>
        TIS_CATEGORY_EXCEPTION_APP = 218,
        /// <summary>
        /// The TI s_ CATEGOR y_ FIL e_ PORTER
        /// </summary>
        TIS_CATEGORY_FILE_PORTER = 51,
        /// <summary>
        /// The TI s_ CATEGOR y_ FILLE r_ APP
        /// </summary>
        TIS_CATEGORY_FILLER_APP = 205,
        /// <summary>
        /// The TI s_ CATEGOR y_ FRE e_ MATC h_ APP
        /// </summary>
        TIS_CATEGORY_FREE_MATCH_APP = 211,
        /// <summary>
        /// The TI s_ CATEGOR y_ FREEDOM
        /// </summary>
        TIS_CATEGORY_FREEDOM = 4,
        /// <summary>
        /// The TI s_ CATEGOR y_ FREEDO m_ MANAGE r_ APP
        /// </summary>
        TIS_CATEGORY_FREEDOM_MANAGER_APP = 232,
        /// <summary>
        /// The TI s_ CATEGOR y_ INDEXIN g_ APP
        /// </summary>
        TIS_CATEGORY_INDEXING_APP = 214,
        /// <summary>
        /// The TI s_ CATEGOR y_ INSTAL l_ APP
        /// </summary>
        TIS_CATEGORY_INSTALL_APP = 243,
        /// <summary>
        /// The TI s_ CATEGOR y_ INTE r_ MODUL e_ COM m_ QUEUE
        /// </summary>
        TIS_CATEGORY_INTER_MODULE_COMM_QUEUE = 145,
        /// <summary>
        /// The TI s_ CATEGOR y_ INTE r_ MODUL e_ COM m_ RECEIVER
        /// </summary>
        TIS_CATEGORY_INTER_MODULE_COMM_RECEIVER = 146,
        /// <summary>
        /// The TI s_ CATEGOR y_ INTE r_ MODUL e_ COM m_ TRANSMITTER
        /// </summary>
        TIS_CATEGORY_INTER_MODULE_COMM_TRANSMITTER = 147,
        /// <summary>
        /// The TI s_ CATEGOR y_ IPE
        /// </summary>
        TIS_CATEGORY_IPE = 1,
        /// <summary>
        /// The TI s_ CATEGOR y_ LEAR n_ APP
        /// </summary>
        TIS_CATEGORY_LEARN_APP = 215,
        /// <summary>
        /// The TI s_ CATEGOR y_ LICENS e_ MANAGER
        /// </summary>
        TIS_CATEGORY_LICENSE_MANAGER = 61,
        /// <summary>
        /// The TI s_ CATEGOR y_ LICENS e_ PARAMETERS
        /// </summary>
        TIS_CATEGORY_LICENSE_PARAMETERS = 60,
        /// <summary>
        /// The TI s_ CATEGOR y_ MANUA l_ PAG e_ I d_ APP
        /// </summary>
        TIS_CATEGORY_MANUAL_PAGE_ID_APP = 207,
        /// <summary>
        /// The TI s_ CATEGOR y_ MOBIL i_ INPU t_ APP
        /// </summary>
        TIS_CATEGORY_MOBILI_INPUT_APP = 206,
        /// <summary>
        /// The TI s_ CATEGOR y_ MOBIL i_ OUTPU t_ APP
        /// </summary>
        TIS_CATEGORY_MOBILI_OUTPUT_APP = 221,
        /// <summary>
        /// The TI s_ CATEGOR y_ MODUL e_ ACTIVATO r_ APP
        /// </summary>
        TIS_CATEGORY_MODULE_ACTIVATOR_APP = 250,
        /// <summary>
        /// The TI s_ CATEGOR y_ OC r_ ANALYZE r_ APP
        /// </summary>
        TIS_CATEGORY_OCR_ANALYZER_APP = 231,
        /// <summary>
        /// The TI s_ CATEGOR y_ OL e_ D b_ WRAPPER
        /// </summary>
        TIS_CATEGORY_OLE_DB_WRAPPER = 50,
        /// <summary>
        /// The TI s_ CATEGOR y_ PAG e_ ORGANIZE r_ APP
        /// </summary>
        TIS_CATEGORY_PAGE_ORGANIZER_APP = 208,
        /// <summary>
        /// The TI s_ CATEGOR y_ PROCESSIN g_ APP
        /// </summary>
        TIS_CATEGORY_PROCESSING_APP = 210,
        /// <summary>
        /// The TI s_ CATEGOR y_ PROCESSIN g_ COMPONENT
        /// </summary>
        TIS_CATEGORY_PROCESSING_COMPONENT = 182,
        /// <summary>
        /// The TI s_ CATEGOR y_ RETRIEVA l_ APP
        /// </summary>
        TIS_CATEGORY_RETRIEVAL_APP = 229,
        /// <summary>
        /// The TI s_ CATEGOR y_ SCA n_ GAT e_ APP
        /// </summary>
        TIS_CATEGORY_SCAN_GATE_APP = 203,
        /// <summary>
        /// The TI s_ CATEGOR y_ SERVE r_ MANAGER
        /// </summary>
        TIS_CATEGORY_SERVER_MANAGER = 128,
        /// <summary>
        /// The TI s_ CATEGOR y_ SERVE r_ MANAGE r_ APP
        /// </summary>
        TIS_CATEGORY_SERVER_MANAGER_APP = 227,
        /// <summary>
        /// The TI s_ CATEGOR y_ SERVE r_ MONITO r_ APP
        /// </summary>
        TIS_CATEGORY_SERVER_MONITOR_APP = 228,
        /// <summary>
        /// The TI s_ CATEGOR y_ SESSIO n_ MANAGE r_ CLIENT
        /// </summary>
        TIS_CATEGORY_SESSION_MANAGER_CLIENT = 168,
        /// <summary>
        /// The TI s_ CATEGOR y_ SESSIO n_ MANAGE r_ SERVER
        /// </summary>
        TIS_CATEGORY_SESSION_MANAGER_SERVER = 110,
        /// <summary>
        /// The TI s_ CATEGOR y_ SESSIO n_ REGISTRY
        /// </summary>
        TIS_CATEGORY_SESSION_REGISTRY = 108,
        /// <summary>
        /// The TI s_ CATEGOR y_ SETU p_ DAT a_ LAYE r_ CLIENT
        /// </summary>
        TIS_CATEGORY_SETUP_DATA_LAYER_CLIENT = 162,
        /// <summary>
        /// The TI s_ CATEGOR y_ SETU p_ DAT a_ LAYE r_ SERVER
        /// </summary>
        TIS_CATEGORY_SETUP_DATA_LAYER_SERVER = 100,
        /// <summary>
        /// The TI s_ CATEGOR y_ STATISTIC s_ AUDI t_ HANDLER
        /// </summary>
        TIS_CATEGORY_STATISTICS_AUDIT_HANDLER = 142,
        /// <summary>
        /// The TI s_ CATEGOR y_ STATISTIC s_ CONTRO l_ SERVER
        /// </summary>
        TIS_CATEGORY_STATISTICS_CONTROL_SERVER = 139,
        /// <summary>
        /// The TI s_ CATEGOR y_ STATISTIC s_ RECORDE r_ CLIENT
        /// </summary>
        TIS_CATEGORY_STATISTICS_RECORDER_CLIENT = 141,
        /// <summary>
        /// The TI s_ CATEGOR y_ STATISTIC s_ RECORDE r_ SERVER
        /// </summary>
        TIS_CATEGORY_STATISTICS_RECORDER_SERVER = 140,
        /// <summary>
        /// The TI s_ CATEGOR y_ STATISTIC s_ REPORTE r_ APP
        /// </summary>
        TIS_CATEGORY_STATISTICS_REPORTER_APP = 230,
        /// <summary>
        /// The TI s_ CATEGOR y_ STATISTIC s_ TYPIS t_ INF o_ GATHER
        /// </summary>
        TIS_CATEGORY_STATISTICS_TYPIST_INFO_GATHER = 143,
        /// <summary>
        /// The TI s_ CATEGOR y_ SUPE r_ ICR
        /// </summary>
        TIS_CATEGORY_SUPER_ICR = 3,
        /// <summary>
        /// The TI s_ CATEGOR y_ TIL e_ APP
        /// </summary>
        TIS_CATEGORY_TILE_APP = 216,
        /// <summary>
        /// The TI s_ CATEGOR y_ TI s_ LOOKU p_ TABLE
        /// </summary>
        TIS_CATEGORY_TIS_LOOKUP_TABLE = 63,
        /// <summary>
        /// The TI s_ CATEGOR y_ UNKNOWN
        /// </summary>
        TIS_CATEGORY_UNKNOWN = 0,
        /// <summary>
        /// The TI s_ CATEGOR y_ USE r_ ACCES s_ CONTRO l_ CLIENT
        /// </summary>
        TIS_CATEGORY_USER_ACCESS_CONTROL_CLIENT = 164,
        /// <summary>
        /// The TI s_ CATEGOR y_ USE r_ ACCES s_ CONTRO l_ SERVER
        /// </summary>
        TIS_CATEGORY_USER_ACCESS_CONTROL_SERVER = 101,
        /// <summary>
        /// The TI s_ CATEGOR y_ VALIDATIO n_ APP
        /// </summary>
        TIS_CATEGORY_VALIDATION_APP = 212,
        /// <summary>
        /// The TI s_ CATEGOR y_ VALIDATIO n_ COMPONENT
        /// </summary>
        TIS_CATEGORY_VALIDATION_COMPONENT = 180,
        /// <summary>
        /// The TI s_ CATEGOR y_ WE b_ EFOR m_ SERVER
        /// </summary>
        TIS_CATEGORY_WEB_EFORM_SERVER = 30,
        /// <summary>
        /// The TI s_ MA x_ CATEGORY
        /// </summary>
        TIS_MAX_CATEGORY = 255,
        /// <summary>
        /// The TI s_ MI n_ CATEGORY
        /// </summary>
        TIS_MIN_CATEGORY = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_IPE_SUBCATEGORIES
    {
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ DAT e_ PROCESSING
        /// </summary>
        TIS_SUBCATEGORY_DATE_PROCESSING = 128,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ ENGIN e_ ATTRIB
        /// </summary>
        TIS_SUBCATEGORY_ENGINE_ATTRIB = 2048,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ ENGIN e_ PARAMS
        /// </summary>
        TIS_SUBCATEGORY_ENGINE_PARAMS = 2,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ FIEL d_ MASK
        /// </summary>
        TIS_SUBCATEGORY_FIELD_MASK = 512,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ FLOA t_ PROCESSING
        /// </summary>
        TIS_SUBCATEGORY_FLOAT_PROCESSING = 256,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ OC r_ ENGINE
        /// </summary>
        TIS_SUBCATEGORY_OCR_ENGINE = 4,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ PAG e_ RECOGNITION
        /// </summary>
        TIS_SUBCATEGORY_PAGE_RECOGNITION = 1,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ PARSIN g_ SERVICES
        /// </summary>
        TIS_SUBCATEGORY_PARSING_SERVICES = 1024,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ POS t_ PROCESSING
        /// </summary>
        TIS_SUBCATEGORY_POST_PROCESSING = 64,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ SUPE r_ OCR
        /// </summary>
        TIS_SUBCATEGORY_SUPER_OCR = 32,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ SUPE r_ SEG
        /// </summary>
        TIS_SUBCATEGORY_SUPER_SEG = 16,
        /// <summary>
        /// The TI s_ SUBCATEGOR y_ VOTING
        /// </summary>
        TIS_SUBCATEGORY_VOTING = 8
    }


    /// <summary>
    /// 
    /// </summary>
    public enum TIS_SOURCE
    {
        /// <summary>
        /// The TI s_ SOURC e_ IMAGE
        /// </summary>
        TIS_SOURCE_IMAGE = 1,
        /// <summary>
        /// The TI s_ SOURC e_ LOGICAL
        /// </summary>
        TIS_SOURCE_LOGICAL = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_TABLE_TYPE
    {
        /// <summary>
        /// The TI s_ COMPUTE r_ LIS t_ TABLE
        /// </summary>
        TIS_COMPUTER_LIST_TABLE = 2,
        /// <summary>
        /// The TI s_ OM r_ TABLE
        /// </summary>
        TIS_OMR_TABLE = 1,
        /// <summary>
        /// The TI s_ REGULA r_ TABLE
        /// </summary>
        TIS_REGULAR_TABLE = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_TAG_ACTION
    {
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ CONTINU e_ CHEC k_ RES t_ O f_ RULES
        /// </summary>
        TIS_TAG_ACTION_CONTINUE_CHECK_REST_OF_RULES = 0,
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ IGNOR e_ RULE s_ I n_ CURREN t_ COLLECTION
        /// </summary>
        TIS_TAG_ACTION_IGNORE_RULES_IN_CURRENT_COLLECTION = 6,
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ IGNOR e_ RULE s_ I n_ CURREN t_ FIELD
        /// </summary>
        TIS_TAG_ACTION_IGNORE_RULES_IN_CURRENT_FIELD = 1,
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ IGNOR e_ RULE s_ I n_ CURREN t_ FIEL d_ GROUP
        /// </summary>
        TIS_TAG_ACTION_IGNORE_RULES_IN_CURRENT_FIELD_GROUP = 2,
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ IGNOR e_ RULE s_ I n_ CURREN t_ FOLDER
        /// </summary>
        TIS_TAG_ACTION_IGNORE_RULES_IN_CURRENT_FOLDER = 5,
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ IGNOR e_ RULE s_ I n_ CURREN t_ FORM
        /// </summary>
        TIS_TAG_ACTION_IGNORE_RULES_IN_CURRENT_FORM = 4,
        /// <summary>
        /// The TI s_ TA g_ ACTIO n_ IGNOR e_ RULE s_ I n_ CURREN t_ PAGE
        /// </summary>
        TIS_TAG_ACTION_IGNORE_RULES_IN_CURRENT_PAGE = 3

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_TEACHING_MODE
    {
        /// <summary>
        /// The TI s_ TEACHIN g_ MOD e_ COMPLETIO n_ AN d_ LEARNING
        /// </summary>
        TIS_TEACHING_MODE_COMPLETION_AND_LEARNING = 2,
        /// <summary>
        /// The TI s_ TEACHIN g_ MOD e_ COMPLETIO n_ ONLY
        /// </summary>
        TIS_TEACHING_MODE_COMPLETION_ONLY = 3,
        /// <summary>
        /// The TI s_ TEACHIN g_ MOD e_ EXCEPTION
        /// </summary>
        TIS_TEACHING_MODE_EXCEPTION = 4,
        /// <summary>
        /// The TI s_ TEACHIN g_ MOD e_ LEARNIN g_ AN d_ COMPLETION
        /// </summary>
        TIS_TEACHING_MODE_LEARNING_AND_COMPLETION = 1,
        /// <summary>
        /// The TI s_ TEACHIN g_ MOD e_ LEARNIN g_ ONLY
        /// </summary>
        TIS_TEACHING_MODE_LEARNING_ONLY = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_TYPE_ENUM
    {

        /// <summary>
        /// The TI s_ BOOL
        /// </summary>
        TIS_BOOL = TypeCode.Boolean,
        /// <summary>
        /// The TI s_ BYTE
        /// </summary>
        TIS_BYTE = TypeCode.Byte,
        /// <summary>
        /// The TI s_ CHAR
        /// </summary>
        TIS_CHAR = TypeCode.Char,
        /// <summary>
        /// The TI s_ DATETIME
        /// </summary>
        TIS_DATETIME = TypeCode.DateTime,
        /// <summary>
        /// The TI s_ DECIMAL
        /// </summary>
        TIS_DECIMAL = TypeCode.Decimal,
        /// <summary>
        /// The TI s_ DOUBLE
        /// </summary>
        TIS_DOUBLE = TypeCode.Double,
        /// <summary>
        /// The TI s_ SHORT
        /// </summary>
        TIS_SHORT = TypeCode.Int16,
        /// <summary>
        /// The TI s_ INT
        /// </summary>
        TIS_INT = TypeCode.Int32,
        /// <summary>
        /// The TI s_ IN T64
        /// </summary>
        TIS_INT64 = TypeCode.Int64,
        /// <summary>
        /// The TI s_ OBJECT
        /// </summary>
        TIS_OBJECT = TypeCode.Object,
        /// <summary>
        /// The TI s_ SMALLINT
        /// </summary>
        TIS_SMALLINT = TypeCode.SByte,
        /// <summary>
        /// The TI s_ SINGLE
        /// </summary>
        TIS_SINGLE = TypeCode.Single,
        /// <summary>
        /// The TI s_ STRING
        /// </summary>
        TIS_STRING = TypeCode.String,
        /// <summary>
        /// The TI s_ WORD
        /// </summary>
        TIS_WORD = TypeCode.UInt16,
        /// <summary>
        /// The TI s_ DWORD
        /// </summary>
        TIS_DWORD = TypeCode.UInt32,
        /// <summary>
        /// The TI s_ QWORD
        /// </summary>
        TIS_QWORD = TypeCode.UInt64

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_USE_IN_MERGE
    {
        /// <summary>
        /// The TI s_ US e_ I n_ MERG e_ AUTO
        /// </summary>
        TIS_USE_IN_MERGE_AUTO = 2,
        /// <summary>
        /// The TI s_ US e_ I n_ MERG e_ FALSE
        /// </summary>
        TIS_USE_IN_MERGE_FALSE = 1,
        /// <summary>
        /// The TI s_ US e_ I n_ MERG e_ TRUE
        /// </summary>
        TIS_USE_IN_MERGE_TRUE = 0

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_VOTING_METHOD
    {
        /// <summary>
        /// The TI s_ VOTIN g_ CUSTOM
        /// </summary>
        TIS_VOTING_CUSTOM = 6,
        /// <summary>
        /// The TI s_ VOTIN g_ EQUALIZER
        /// </summary>
        TIS_VOTING_EQUALIZER = 5,
        /// <summary>
        /// The TI s_ VOTIN g_ GUESS
        /// </summary>
        TIS_VOTING_GUESS = 3,
        /// <summary>
        /// The TI s_ VOTIN g_ NORMAL
        /// </summary>
        TIS_VOTING_NORMAL = 1,
        /// <summary>
        /// The TI s_ VOTIN g_ ORDER
        /// </summary>
        TIS_VOTING_ORDER = 2,
        /// <summary>
        /// The TI s_ VOTIN g_ SAFE
        /// </summary>
        TIS_VOTING_SAFE = 0,
        /// <summary>
        /// The TI s_ VOTIN g_ SUPER
        /// </summary>
        TIS_VOTING_SUPER = 4

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_FULL_PAGE_VOTING_METHOD
    {
        /// <summary>
        /// The XVOTER
        /// </summary>
        XVOTER
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_TABLE_AUTORECOGNIZE_ACTION
    {
        /// <summary>
        /// The TI s_ TABL e_ AUTORECOGNIZ e_ ALWAYS
        /// </summary>
        TIS_TABLE_AUTORECOGNIZE_ALWAYS = 0,
        /// <summary>
        /// The TI s_ TABL e_ AUTORECOGNIZ e_ ASKUSER
        /// </summary>
        TIS_TABLE_AUTORECOGNIZE_ASKUSER = 2,
        /// <summary>
        /// The TI s_ TABL e_ AUTORECOGNIZ e_ NEVER
        /// </summary>
        TIS_TABLE_AUTORECOGNIZE_NEVER = 1
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_METATAG_SOURCE
    {
        /// <summary>
        /// The SPECIA l_ TAG
        /// </summary>
        SPECIAL_TAG = 0,
        /// <summary>
        /// The PARAMETER
        /// </summary>
        PARAMETER = 1,
        /// <summary>
        /// The USE r_ TAG
        /// </summary>
        USER_TAG = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_METATAG_TYPE
    {
        /// <summary>
        /// The M t_ BOOL
        /// </summary>
        MT_BOOL = TypeCode.Boolean,
        /// <summary>
        /// The M t_ STRING
        /// </summary>
        MT_STRING = TypeCode.String,
        /// <summary>
        /// The M t_ IN t8
        /// </summary>
        MT_INT8 = TypeCode.Byte,
        /// <summary>
        /// The M t_ IN T16
        /// </summary>
        MT_INT16 = TypeCode.Int16,
        /// <summary>
        /// The M t_ IN T32
        /// </summary>
        MT_INT32 = TypeCode.Int32,
        /// <summary>
        /// The M t_ IN T64
        /// </summary>
        MT_INT64 = TypeCode.Int64,
        /// <summary>
        /// The M t_ SINGLE
        /// </summary>
        MT_SINGLE = TypeCode.Single,
        /// <summary>
        /// The M t_ DOUBLE
        /// </summary>
        MT_DOUBLE = TypeCode.Double,
        /// <summary>
        /// The M t_ DECIMAL
        /// </summary>
        MT_DECIMAL = TypeCode.Decimal,
        /// <summary>
        /// The M t_ DATETIME
        /// </summary>
        MT_DATETIME = TypeCode.DateTime
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TIS_OCR_ENGINE_TYPE
    {
        /// <summary>
        /// The FIELD
        /// </summary>
        FIELD = 0,
        /// <summary>
        /// The PAGE
        /// </summary>
        PAGE = 1
    }

    /// <summary>
    /// Represents a set of possible collection actions
    /// </summary>
    /// <remarks>
    /// These actions are supported by Controller station events
    /// </remarks>
    public enum TIS_MANAGE_COLLECTION_ACTION
    {
        /// <summary>
        /// Delete collection
        /// </summary>
        DELETE = 0,
        /// <summary>
        /// Put collection on hold
        /// </summary>
        HOLD = 1,
        /// <summary>
        /// Release collection previously set on hold
        /// </summary>
        RELEASE = 2,
        /// <summary>
        /// Move collection to another queue
        /// </summary>
        MOVE = 3
    }
}
