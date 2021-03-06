﻿using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Globalization;
namespace Mono.TextTemplating.Utility.EntityFramework
{

    /// <summary>
    /// Responsible for collecting together the actual method parameters
    /// and the parameters that need to be sent to the Execute method.
    /// </summary>
    public class FunctionImportParameter
    {
        public FunctionParameter Source { get; set; }
        public string RawFunctionParameterName { get; set; }
        public string FunctionParameterName { get; set; }
        public string FunctionParameterType { get; set; }
        public string LocalVariableName { get; set; }
        public string RawClrTypeName { get; set; }
        public string ExecuteParameterName { get; set; }
        public string EsqlParameterName { get; set; }
        public bool NeedsLocalVariable { get; set; }
        public bool IsNullableOfT { get; set; }


        /// <summary>
        /// Creates a set of FunctionImportParameter objects from the parameters passed in.
        /// </summary>
        public static IEnumerable<FunctionImportParameter> Create(IEnumerable<FunctionParameter> parameters, CodeGenerationTools code, MetadataTools ef)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (ef == null)
            {
                throw new ArgumentNullException("ef");
            }

            UniqueIdentifierService unique = new UniqueIdentifierService();
            List<FunctionImportParameter> importParameters = new List<FunctionImportParameter>();
            foreach (FunctionParameter parameter in parameters)
            {
                FunctionImportParameter importParameter = new FunctionImportParameter();
                importParameter.Source = parameter;
                importParameter.RawFunctionParameterName = unique.AdjustIdentifier(code.CamelCase(parameter.Name));
                importParameter.FunctionParameterName = code.Escape(importParameter.RawFunctionParameterName);
                if (parameter.Mode == ParameterMode.In)
                {
                    importParameter.NeedsLocalVariable = true;
                    importParameter.FunctionParameterType = code.Escape(parameter.TypeUsage);
                    importParameter.EsqlParameterName = parameter.Name;
                    Type clrType = ef.ClrType(parameter.TypeUsage);
                    importParameter.RawClrTypeName = code.Escape(clrType);
                    importParameter.IsNullableOfT = clrType.IsValueType;
                }
                else
                {
                    importParameter.NeedsLocalVariable = false;
                    importParameter.FunctionParameterType = "ObjectParameter";
                    importParameter.ExecuteParameterName = importParameter.FunctionParameterName;
                }
                importParameters.Add(importParameter);
            }

            // we save the local parameter uniquification for a second pass to make the visible parameters
            // as pretty and sensible as possible
            for (int i = 0; i < importParameters.Count; i++)
            {
                FunctionImportParameter importParameter = importParameters[i];
                if (importParameter.NeedsLocalVariable)
                {
                    importParameter.LocalVariableName = unique.AdjustIdentifier(importParameter.RawFunctionParameterName + "Parameter");
                    importParameter.ExecuteParameterName = importParameter.LocalVariableName;
                }
            }

            return importParameters;
        }

        //
        // Class to create unique variables within the same scope
        //
        private sealed class UniqueIdentifierService
        {
            private readonly HashSet<string> _knownIdentifiers;

            public UniqueIdentifierService()
            {
                _knownIdentifiers = new HashSet<string>(StringComparer.Ordinal);
            }

            /// <summary>
            /// Given an identifier, makes it unique within the scope by adding
            /// a suffix (1, 2, 3, ...), and returns the adjusted identifier.
            /// </summary>
            public string AdjustIdentifier(string identifier)
            {
                // find a unique name by adding suffix as necessary
                int numberOfConflicts = 0;
                string adjustedIdentifier = identifier;

                while (!_knownIdentifiers.Add(adjustedIdentifier))
                {
                    ++numberOfConflicts;
                    adjustedIdentifier = identifier + numberOfConflicts.ToString(CultureInfo.InvariantCulture);
                }

                return adjustedIdentifier;
            }
        }

        private string FunctionImportTypeName(FunctionParameter parameter)
        {
            return parameter.Mode == ParameterMode.In ? parameter.TypeUsage.EdmType.Name : "ObjectParameter";
        }
    }
}