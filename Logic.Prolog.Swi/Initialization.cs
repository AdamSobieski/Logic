using System;
using System.IO;

namespace Logic.Prolog.Swi.Initialization
{
//    swipl: Usage:
//    1) swipl[options] prolog-file... [--arg...]
//    2) swipl[options][-o executable] -c prolog-file...
//    3) swipl --help Display this message (also -h)
//    4) swipl --version Display version information
//    4) swipl --arch Display architecture
//    6) swipl --dump-runtime-variables [=format]
//    Dump link info in sh(1) format

//Options:
//    -x state Start from state (must be first)
//    -g goal Run goal (may be repeated)
//    -t toplevel Toplevel goal
//    -f file User initialisation file
//    -F file Site initialisation file
//    -l file Script source file
//    -s file Script source file
//    -p alias=path Define file search path 'alias'
//    -O Optimised compilation
//    --tty [=bool]             (Dis)allow tty control
//    --signals [=bool]
//    Do (not) modify signal handling
//    --threads [=bool]
//    Do (not) allow for threads
//    --debug [=bool]
//    Do (not) generate debug info
//    --quiet [=bool] (-q)      Do (not) suppress informational messages
//    --traditional Disable extensions of version 7
//    --home=DIR Use DIR as SWI-Prolog home
//    --stack_limit=size [BKMG]
//    Specify maximum size of Prolog stacks
//    --table_space=size [BKMG]
//    Specify maximum size of SLG tables
//    --pldoc [=port]
//    Start PlDoc server [at port]
//    --win_app Behave as Windows application

//Boolean options may be written as --name=bool, --name, --no-name or --noname

    public class SwiPrologInitializationSettings
    {
        public SwiPrologInitializationSettings()
        {
            m_homedirectory = Environment.GetEnvironmentVariable("SWI_HOME_DIR");
            m_homedirectory_isdefault = true;

            m_bindirectory = null;
            m_bindirectory_isdefault = true;

            m_setenvironmentvariable = false;
            m_setenvironmentvariable_isdefault = true;

            m_prependpath = false;
            m_prependpath_isdefault = true;
        }

        string m_homedirectory;
        bool m_homedirectory_isdefault;
        string m_bindirectory;
        bool m_bindirectory_isdefault;
        bool m_setenvironmentvariable;
        bool m_setenvironmentvariable_isdefault;
        bool m_prependpath;
        bool m_prependpath_isdefault;

        public string HomeDirectory
        {
            get
            {
                return m_homedirectory;
            }
            set
            {
                m_homedirectory = value;
                m_homedirectory_isdefault = false;
            }
        }
        public string BinaryDirectory
        {
            get
            {
                if (m_bindirectory_isdefault == true)
                {
                    return m_homedirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "bin";
                }
                else
                {
                    return m_bindirectory;
                }
            }
            set
            {
                m_bindirectory = value;
                m_bindirectory_isdefault = false;
            }
        }

        public bool SetHomeDirectoryEnvironmentVariable
        {
            get
            {
                return m_setenvironmentvariable;
            }
            set
            {
                m_setenvironmentvariable = value;
                m_setenvironmentvariable_isdefault = false;
            }
        }
        public bool PrependBinaryDirectoryToPath
        {
            get
            {
                return m_prependpath;
            }
            set
            {
                m_prependpath = value;
                m_prependpath_isdefault = false;
            }
        }



        internal bool SwiPrologHomeDirectoryIsDefault
        {
            get
            {
                return m_bindirectory_isdefault;
            }
        }
        internal bool SetEnvironmentVariableIsDefault
        {
            get
            {
                return m_setenvironmentvariable_isdefault;
            }
        }
        internal bool PrependSwiBinaryDirectoryToPathIsDefault
        {
            get
            {
                return m_prependpath_isdefault;
            }
        }



        internal string[] GenerateParameters()
        {
            return new string[] { "-q", "-O", "--signals=false", "--debug=false" };
        }
    }
}
