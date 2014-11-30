using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{
    [ComVisible(false)]
    public class CustomSection : ConfigurationSection
    {
        private object m_data;
        private string m_typeName;

        public CustomSection()
        {
        }

        protected override object GetRuntimeObject()
        {
            return m_data;
        }

        public object Data
        {
            get { return m_data; }
            set
            {
                m_data = value;
                Type type = m_data.GetType();
                m_typeName = type.FullName + ", " + type.Assembly.FullName;
            }
        }

        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            if (!reader.Read() || (reader.NodeType != XmlNodeType.Element))
            {
                ConfigurationErrorsException exc = new ConfigurationErrorsException(
                    "Configuration reader expected to find an element", reader);

                Log.Write(Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    exc.Message);

                throw exc;
            }
            using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
            {
                this.DeserializeElement(reader, false);
            }
        }

        /// <summary>
        /// Serializes the configuration section to an XML string representation.
        /// </summary>
        /// <param name="parentElement">The parent element of this element.</param>
        /// <param name="name">The name of the section.</param>
        /// <param name="saveMode">The mode to use for saving.</param>
        /// <returns>The string representation of the section.</returns>
        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            using (StringWriter sWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
            {
                XmlTextWriter xWriter = new XmlTextWriter(sWriter);
                xWriter.Formatting = Formatting.Indented;
                xWriter.Indentation = 4;
                xWriter.IndentChar = ' ';
                this.SerializeToXmlElement(xWriter, name);
                xWriter.Flush();
                return sWriter.ToString();
            }
        }

        protected override bool SerializeElement(
              XmlWriter writer,
              bool serializeCollectionKey)
        {

            if (writer != null && this != null)
            {
                DataContractSerializer serializer = new DataContractSerializer(m_data.GetType());

                writer.WriteAttributeString("Type", m_typeName);

                serializer.WriteObject(writer, m_data);
            }
            return true;
        }

        /// <summary>
        /// Serializes the section into the configuration file.
        /// </summary>
        /// <param name="writer">The writer to use for serializing the class.</param>
        /// <param name="elementName">The name of the configuration section.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
        {
            if (writer == null)
                return false;
            writer.WriteStartElement(elementName);
            bool success = true;

            success = SerializeElement(writer, false);

            writer.WriteEndElement();
            return success;
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            try
            {
                reader.MoveToContent();

                // Check for invalid usage
                if (reader.AttributeCount > 1)
                    throw new ConfigurationErrorsException("Only a single type is allowed.");
                if (reader.AttributeCount == 0)
                    throw new ConfigurationErrorsException("A type or is required.");

                DeserializeData(reader);

                reader.ReadEndElement();
            }
            catch (Exception exc)
            {
                Log.Write(Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    " Error : {0}", exc.Message);
            }
        }

        //<summary>
        //Deserializes the data from the reader.
        //</summary>
        //<param name="reader">The XmlReader containing the serialized data.</param>
        private void DeserializeData(XmlReader reader)
        {
            m_typeName = reader.GetAttribute("Type");

            Type dataType = Type.GetType(this.m_typeName);

            reader.Read();

            reader.MoveToContent();

            DataContractSerializer serializer = new DataContractSerializer(dataType);

            using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
            {
                this.Data = serializer.ReadObject(reader);
            }
        }
    }
}
