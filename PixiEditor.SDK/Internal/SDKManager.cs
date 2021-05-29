﻿using PixiEditor.SDK.FileParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PixiEditor.SDK
{
    internal class SDKManager
    {
        public List<Extension> Extensions { get; } = new();

        public FileParserList Parsers { get; } = new();

        public List<string> SupportedFileExtensions { get; } = new();

        public void AddExtension(Extension extension)
        {
            Extensions.Add(extension);
        }

        public ExtensionLoadingResult LoadExtensions(string extensionLocation)
        {
            List<ExtensionLoadingException> extensionExceptions = new();
            List<Assembly> loadedAssemblies = new();

            if (!Directory.Exists(extensionLocation))
            {
                Directory.CreateDirectory(extensionLocation);
            }

            foreach (string path in Directory.EnumerateFiles(extensionLocation, "*.dll"))
            {
                try
                {
                    loadedAssemblies.Add(Assembly.LoadFrom(path));
                }
                catch (Exception e)
                {
                    extensionExceptions.Add(new ExtensionLoadingException(path, "Error while trying to load extension", e));
                }
            }

            foreach (Assembly assembly in loadedAssemblies)
            {
                try
                {
                    Extension extension = LoadExtensionFromAssembly(assembly);
                    extension.ExtensionPath = assembly.Location;

                    Extensions.Add(extension);
                }
                catch (Exception e)
                {
                    extensionExceptions.Add(new ExtensionLoadingException(assembly.Location, "Error while trying to initialize extension", e));
                }
            }

            return new ExtensionLoadingResult(extensionExceptions.ToArray());
        }

        public void SetupExtensions()
        {
            List<Extension> extensions = new List<Extension>(Extensions);

            foreach (Extension extension in extensions)
            {
                try
                {
                    ExtensionLoadingInformation information = new(extension);
                    extension.Load(information);

                    foreach (DocumentParserInfo fileParserInformation in information.DocumentParsers)
                    {
                        foreach (string fileExtension in fileParserInformation.SupportedFileExtensions)
                        {
                            Parsers.DocumentParsers.Add(fileExtension, fileParserInformation);

                            if (!SupportedFileExtensions.Contains(fileExtension))
                            {
                                SupportedFileExtensions.Add(fileExtension);
                            }
                        }
                    }

                    foreach (ImageParserInfo fileParserInformation in information.ImageParsers)
                    {
                        foreach (string fileExtension in fileParserInformation.SupportedFileExtensions)
                        {
                            Parsers.ImageParsers.Add(fileExtension, fileParserInformation);

                            if (!SupportedFileExtensions.Contains(fileExtension))
                            {
                                SupportedFileExtensions.Add(fileExtension);
                            }
                        }
                    }
                }
                catch
                {
                    Extensions.Remove(extension);
                }
            }
        }

        private static Extension LoadExtensionFromAssembly(Assembly assembly)
        {
            PixiEditorExtensionAttribute attribute = assembly.GetCustomAttribute<PixiEditorExtensionAttribute>();

            ConstructorInfo info = attribute.ExtensionType.GetConstructor(Type.EmptyTypes);

            Extension extension = info.Invoke(null) as Extension;
            extension.ExtensionPath = assembly.Location;

            return extension;
        }
    }
}