using System;
using System.Collections.Generic;
using System.Linq;
using DotNetXtensions;
using MTI = DotNetXtensions.MimeTypeInfo;

namespace DotNetXtensions
{
	public static partial class BasicMimeTypesX
	{
		static Dictionary<BasicMimeType, MTI> MTIDict_ByEnums;
		static Dictionary<string, MTI> MTIDict_ByMimeNames;
		static Dictionary<string, MTI> MTIDict_ByExtensions;


		static BasicMimeTypesX()
		{
			MTIDict_ByEnums = AllTypes.ToDictionary(v => v.MimeType, v => v);
			MTIDict_ByMimeNames = AllTypes.ToDictionary(v => v.MimeTypeString, v => v);
			MTIDict_ByExtensions = AllTypes.Where(v => v.Extension.NotNulle()).ToDictionary(v => v.Extension, v => v);

			for(int i = 0; i < AllTypes.Length; i++) {
				MTI mti = AllTypes[i];

				if(mti.ExtraMimeStrings.NotNulle()) {
					foreach(string extMimeStr in mti.ExtraMimeStrings) {
						if(!MTIDict_ByMimeNames.ContainsKey(extMimeStr))
							MTIDict_ByMimeNames.Add(extMimeStr, mti);
					}
				}

				if(mti.ExtraExtensions.NotNulle()) {
					foreach(string ext in mti.ExtraExtensions) {
						if(!MTIDict_ByExtensions.ContainsKey(ext))
							MTIDict_ByExtensions.Add(ext, mti);
					}
				}
			}
		}

		// --- +++ ---

		// let's keep the following here, as it's core to the base static types
		// we have, regetting them for the outside world but in a safe way

		/// <summary>
		/// Makes a deep copy of the internal <see cref="MTIDict_ByEnums"/> dictionary.
		/// </summary>
		public static Dictionary<BasicMimeType, MTI> GetMimeTypeInfoDictKeyedByEnums()
			=> MTIDict_ByEnums.ToDictionary(vv => vv.Key, vv => vv.Value?.Copy());

		/// <summary>
		/// Makes a deep copy of the internal <see cref="MTIDict_ByMimeNames"/> dictionary.
		/// </summary>
		public static Dictionary<string, MTI> GetMimeTypeInfoDictKeyedByMimeNames()
			=> MTIDict_ByMimeNames.ToDictionary(vv => vv.Key, vv => vv.Value?.Copy());

		/// <summary>
		/// Makes a deep copy of the internal <see cref="MTIDict_ByExtensions"/> dictionary.
		/// </summary>
		public static Dictionary<string, MTI> GetMimeTypeInfoDictKeyedByExtensions()
			=> MTIDict_ByExtensions.ToDictionary(vv => vv.Key, vv => vv.Value?.Copy());

	}
}
