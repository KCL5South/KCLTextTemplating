using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace Mono.TextTemplating.Utility.EntityFramework
{

    /// <summary>
    /// Responsible for marking the various sections of the generation,
    /// so they can be split up into separate files
    /// </summary>
    public class EntityFrameworkTemplateFileManager
    {
        /// <summary>
        /// Creates the VsEntityFrameworkTemplateFileManager if VS is detected, otherwise
        /// creates the file system version.
        /// </summary>
        public static EntityFrameworkTemplateFileManager Create(object textTransformation)
        {
            DynamicTextTransformation transformation = DynamicTextTransformation.Create(textTransformation);
            IDynamicHost host = transformation.Host;

            return new EntityFrameworkTemplateFileManager(transformation);
        }

        private sealed class Block
        {
            public String Name;
            public int Start, Length;
        }

        private readonly List<Block> files = new List<Block>();
        private readonly Block footer = new Block();
        private readonly Block header = new Block();
        private readonly DynamicTextTransformation _textTransformation;

        // reference to the GenerationEnvironment StringBuilder on the
        // TextTransformation object
        private readonly StringBuilder _generationEnvironment;

        private Block currentBlock;

        /// <summary>
        /// Initializes an EntityFrameworkTemplateFileManager Instance  with the
        /// TextTransformation (T4 generated class) that is currently running
        /// </summary>
        private EntityFrameworkTemplateFileManager(object textTransformation)
        {
            if (textTransformation == null)
            {
                throw new ArgumentNullException("textTransformation");
            }

            _textTransformation = DynamicTextTransformation.Create(textTransformation);
            _generationEnvironment = _textTransformation.GenerationEnvironment;
        }

        /// <summary>
        /// Marks the end of the last file if there was one, and starts a new
        /// and marks this point in generation as a new file.
        /// </summary>
        public void StartNewFile(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            CurrentBlock = new Block { Name = name };
        }

        public void StartFooter()
        {
            CurrentBlock = footer;
        }

        public void StartHeader()
        {
            CurrentBlock = header;
        }

        public void EndBlock()
        {
            if (CurrentBlock == null)
            {
                return;
            }

            CurrentBlock.Length = _generationEnvironment.Length - CurrentBlock.Start;

            if (CurrentBlock != header && CurrentBlock != footer)
            {
                files.Add(CurrentBlock);
            }

            currentBlock = null;
        }

        /// <summary>
        /// Produce the template output files.
        /// </summary>
        public virtual IEnumerable<string> Process(bool split = true)
        {
            var generatedFileNames = new List<string>();

            if (split)
            {
                EndBlock();

                var headerText = _generationEnvironment.ToString(header.Start, header.Length);
                var footerText = _generationEnvironment.ToString(footer.Start, footer.Length);
                var outputPath = Path.GetDirectoryName(_textTransformation.Host.TemplateFile);

                files.Reverse();

                foreach (var block in files)
                {
                    var fileName = Path.Combine(outputPath, block.Name);
                    var content = headerText + _generationEnvironment.ToString(block.Start, block.Length) + footerText;

                    generatedFileNames.Add(fileName);
                    CreateFile(fileName, content);
                    _generationEnvironment.Remove(block.Start, block.Length);
                }
            }

            return generatedFileNames;
        }

        protected virtual void CreateFile(string fileName, string content)
        {
            if (IsFileContentDifferent(fileName, content))
            {
                File.WriteAllText(fileName, content);
            }
        }

        protected bool IsFileContentDifferent(String fileName, string newContent)
        {
            return !(File.Exists(fileName) && File.ReadAllText(fileName) == newContent);
        }

        private Block CurrentBlock
        {
            get { return currentBlock; }
            set
            {
                if (CurrentBlock != null)
                {
                    EndBlock();
                }

                if (value != null)
                {
                    value.Start = _generationEnvironment.Length;
                }

                currentBlock = value;
            }
        }
    }
}