using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions.Globalization
{
	/// <summary>
	/// Class containing world country names.
	/// </summary>
	public static class GeoNames
	{
		/// <summary>
		/// Country names.
		/// </summary>
		public static readonly string[] GeoCountries;

		/// <summary>
		/// US and Canadian state names.
		/// </summary>
		public static readonly string[] USCanadaStates;

		/// <summary>
		/// Country names sorted. This is useful when conducting a binary search.
		/// </summary>
		public static readonly string[] GeoCountriesOrdered;

		/// <summary>
		/// US and Canadian state names ordered. This is useful when conducting a binary search.
		/// </summary>
		public static readonly string[] USCanadaStatesOrdered;

		/// <summary>
		/// Country names with a few of the common countries many of us have to deal with on top.
		/// You can change these values (or simply never use this!) by calling SetCountriesTopped.
		/// </summary>
		public static string[] GeoCountriesTopped;

		/// <summary>
		/// A dictionary of country names <i>keyed by the full country name</i>, with the abbreviation as the value.
		/// So this allows one to lookup the abbreviation for a country when they already have the full country name.
		/// </summary>
		public static readonly IDictionary<string, string> GeoCountriesDictByFullName;

		/// <summary>
		/// A dictionary of state names <i>keyed by the full state name</i>, with the abbreviation as the value.
		/// So this allows one to lookup the abbreviation for a state when they already have the full state name.
		/// </summary>
		public static readonly IDictionary<string, string> USCanadaStatesDictByFullName;



		// STATIC CONSTRUCTOR

		static GeoNames()
		{
			// ----- COUNTRIES -----

			// NOTE: GeoCountriesDict is foundational one (already inited)

			GeoCountries = GeoCountriesByAbbreviationDict.Values.ToArray().Sort();
			GeoCountriesOrdered = GeoCountries.OrderBy(s => s).ToArray();
			GeoCountriesDictByFullName = GeoCountriesByAbbreviationDict.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

			// important, init this after these others (at least the country inits)
			GeoCountriesTopped = GetCountriesWithTheseOnTop("US", "CA", "UK", "DE", "AU");


			// ----- STATES -----

			// NOTE: USCanadaStatesDict is foundational one (already inited)

			USCanadaStates = USCanadaStatesByAbbreviationDict.Values.ToArray().Sort();
			USCanadaStatesOrdered = USCanadaStates.OrderBy(s => s).ToArray();
			USCanadaStatesDictByFullName = USCanadaStatesByAbbreviationDict.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);


			// --- Init ---

			AllCountryNamesDict = new Dictionary<string, GeoCountry>(700, StringComparer.OrdinalIgnoreCase);
			InitCountryEnum(ref AllCountryNamesDict);

			AllUSCanadaStateNamesDict = new Dictionary<string, USCanadaState>(200, StringComparer.OrdinalIgnoreCase);
			InitUSCanadaStateEnum(ref AllUSCanadaStateNamesDict);

			// AllCountryNamesDict = new Dictionary<string, GeoCountry>(250, StringComparer.OrdinalIgnoreCase);
			// AllUSCanadaStateNamesDict = new Dictionary<string, USCanadaState>(250, StringComparer.OrdinalIgnoreCase);
		}




		// ===== ALL THE INFORMATION COMES FROM THESE TWO DICTIONARIES ======

		/// <summary>
		/// A dictionary of country names <i>keyed by the country abbreviation</i>, with the full country name as the value.
		/// So this allows one to lookup the abbreviation for a country when they already have the full name.
		/// <remarks>
		/// Information was initially based on: http://www.paladinsoftware.com/Generic/countries.htm (Nov 2013)
		/// </remarks>
		/// </summary>
		public static readonly IDictionary<string, string> GeoCountriesByAbbreviationDict = new Dictionary<string, string>(250, StringComparer.OrdinalIgnoreCase)
		{
			#region ===== COUNTRIES =====

			{ "AF", "Afghanistan" },
			{ "AL", "Albania" },
			{ "DZ", "Algeria" },
			{ "AS", "American Samoa" },
			{ "AD", "Andorra" },
			{ "AO", "Angola" },
			{ "AI", "Anguilla" },
			{ "AQ", "Antarctica" },
			{ "AG", "Antigua and Barbuda" },
			{ "AR", "Argentina" },
			{ "AM", "Armenia" },
			{ "AW", "Aruba" },
			{ "AU", "Australia" },
			{ "AT", "Austria" },
			{ "AZ", "Azerbaijan" },
			{ "BS", "Bahamas" },
			{ "BH", "Bahrain" },
			{ "BD", "Bangladesh" },
			{ "BB", "Barbados" },
			{ "BY", "Belarus" },
			{ "BE", "Belgium" },
			{ "BZ", "Belize" },
			{ "BJ", "Benin" },
			{ "BM", "Bermuda" },
			{ "BT", "Bhutan" },
			{ "BO", "Bolivia" },
			{ "BA", "Bosnia and Herzegovina" },
			{ "BW", "Botswana" },
			{ "BV", "Bouvet Island" },
			{ "BR", "Brazil" },
			{ "IO", "British Indian Ocean Territory" },
			{ "BN", "Brunei Darussalam" },
			{ "BG", "Bulgaria" },
			{ "BF", "Burkina Faso" },
			{ "BI", "Burundi" },
			{ "KH", "Cambodia" },
			{ "CM", "Cameroon" },
			{ "CA", "Canada" },
			{ "CV", "Cape Verde" },
			{ "KY", "Cayman Islands" },
			{ "CF", "Central African Republic" },
			{ "TD", "Chad" },
			{ "CL", "Chile" },
			{ "CN", "China" },
			{ "CX", "Christmas Island" },
			{ "CC", "Cocos (Keeling Islands)" },
			{ "CO", "Colombia" },
			{ "KM", "Comoros" },
			{ "CG", "Congo" },
			{ "CK", "Cook Islands" },
			{ "CR", "Costa Rica" },
			{ "CI", "Cote D'Ivoire (Ivory Coast)" },
			{ "HR", "Croatia (Hrvatska)" },
			{ "CU", "Cuba" },
			{ "CY", "Cyprus" },
			{ "CZ", "Czech Republic" },
			{ "DK", "Denmark" },
			{ "DJ", "Djibouti" },
			{ "DM", "Dominica" },
			{ "DO", "Dominican Republic" },
			{ "TP", "East Timor" },
			{ "EC", "Ecuador" },
			{ "EG", "Egypt" },
			{ "SV", "El Salvador" },
			{ "GQ", "Equatorial Guinea" },
			{ "ER", "Eritrea" },
			{ "EE", "Estonia" },
			{ "ET", "Ethiopia" },
			{ "FK", "Falkland Islands (Malvinas)" },
			{ "FO", "Faroe Islands" },
			{ "FJ", "Fiji" },
			{ "FI", "Finland" },
			{ "FR", "France" },
			{ "FX", "France, Metropolitan" },
			{ "GF", "French Guiana" },
			{ "PF", "French Polynesia" },
			{ "TF", "French Southern Territories" },
			{ "GA", "Gabon" },
			{ "GM", "Gambia" },
			{ "GE", "Georgia" },
			{ "DE", "Germany" },
			{ "GH", "Ghana" },
			{ "GI", "Gibraltar" },
			{ "GR", "Greece" },
			{ "GL", "Greenland" },
			{ "GD", "Grenada" },
			{ "GP", "Guadeloupe" },
			{ "GU", "Guam" },
			{ "GT", "Guatemala" },
			{ "GN", "Guinea" },
			{ "GW", "Guinea-Bissau" },
			{ "GY", "Guyana" },
			{ "HT", "Haiti" },
			{ "HM", "Heard and McDonald Islands" },
			{ "HN", "Honduras" },
			{ "HK", "Hong Kong" },
			{ "HU", "Hungary" },
			{ "IS", "Iceland" },
			{ "IN", "India" },
			{ "ID", "Indonesia" },
			{ "IR", "Iran" },
			{ "IQ", "Iraq" },
			{ "IE", "Ireland" },
			{ "IL", "Israel" },
			{ "IT", "Italy" },
			{ "JM", "Jamaica" },
			{ "JP", "Japan" },
			{ "JO", "Jordan" },
			{ "KZ", "Kazakhstan" },
			{ "KE", "Kenya" },
			{ "KI", "Kiribati" },
			{ "KW", "Kuwait" },
			{ "KG", "Kyrgyzstan" },
			{ "LA", "Laos" },
			{ "LV", "Latvia" },
			{ "LB", "Lebanon" },
			{ "LS", "Lesotho" },
			{ "LR", "Liberia" },
			{ "LY", "Libya" },
			{ "LI", "Liechtenstein" },
			{ "LT", "Lithuania" },
			{ "LU", "Luxembourg" },
			{ "MO", "Macau" },
			{ "MK", "Macedonia" },
			{ "MG", "Madagascar" },
			{ "MW", "Malawi" },
			{ "MY", "Malaysia" },
			{ "MV", "Maldives" },
			{ "ML", "Mali" },
			{ "MT", "Malta" },
			{ "MH", "Marshall Islands" },
			{ "MQ", "Martinique" },
			{ "MR", "Mauritania" },
			{ "MU", "Mauritius" },
			{ "YT", "Mayotte" },
			{ "MX", "Mexico" },
			{ "FM", "Micronesia" },
			{ "MD", "Moldova" },
			{ "MC", "Monaco" },
			{ "MN", "Mongolia" },
			{ "MS", "Montserrat" },
			{ "MA", "Morocco" },
			{ "MZ", "Mozambique" },
			{ "MM", "Myanmar" },
			{ "NA", "Namibia" },
			{ "NR", "Nauru" },
			{ "NP", "Nepal" },
			{ "NL", "Netherlands" },
			{ "AN", "Netherlands Antilles" },
			{ "NC", "New Caledonia" },
			{ "NZ", "New Zealand" },
			{ "NI", "Nicaragua" },
			{ "NE", "Niger" },
			{ "NG", "Nigeria" },
			{ "NU", "Niue" },
			{ "NF", "Norfolk Island" },
			{ "KP", "North Korea" },
			{ "MP", "Northern Mariana Islands" },
			{ "NO", "Norway" },
			{ "OM", "Oman" },
			{ "PK", "Pakistan" },
			{ "PW", "Palau" },
			{ "PA", "Panama" },
			{ "PG", "Papua New Guinea" },
			{ "PY", "Paraguay" },
			{ "PE", "Peru" },
			{ "PH", "Philippines" },
			{ "PN", "Pitcairn" },
			{ "PL", "Poland" },
			{ "PT", "Portugal" },
			{ "PR", "Puerto Rico" },
			{ "QA", "Qatar" },
			{ "RE", "Reunion" },
			{ "RO", "Romania" },
			{ "RU", "Russian Federation" },
			{ "RW", "Rwanda" },
			{ "GS", "S. Georgia and S. Sandwich Isls." },
			{ "KN", "Saint Kitts and Nevis" },
			{ "LC", "Saint Lucia" },
			{ "VC", "Saint Vincent and The Grenadines" },
			{ "WS", "Samoa" },
			{ "SM", "San Marino" },
			{ "ST", "Sao Tome and Principe" },
			{ "SA", "Saudi Arabia" },
			{ "SN", "Senegal" },
			{ "SC", "Seychelles" },
			{ "SL", "Sierra Leone" },
			{ "SG", "Singapore" },
			{ "SK", "Slovak Republic" },
			{ "SI", "Slovenia" },
			{ "SB", "Solomon Islands" },
			{ "SO", "Somalia" },
			{ "ZA", "South Africa" },
			{ "KR", "Sourth Korea" },
			{ "ES", "Spain" },
			{ "LK", "Sri Lanka" },
			{ "SH", "St. Helena" },
			{ "PM", "St. Pierre and Miquelon" },
			{ "SD", "Sudan" },
			{ "SR", "Suriname" },
			{ "SJ", "Svalbard and Jan Mayen Islands" },
			{ "SZ", "Swaziland" },
			{ "SE", "Sweden" },
			{ "CH", "Switzerland" },
			{ "SY", "Syria" },
			{ "TW", "Taiwan" },
			{ "TJ", "Tajikistan" },
			{ "TZ", "Tanzania" },
			{ "TH", "Thailand" },
			{ "TG", "Togo" },
			{ "TK", "Tokelau" },
			{ "TO", "Tonga" },
			{ "TT", "Trinidad and Tobago" },
			{ "TN", "Tunisia" },
			{ "TR", "Turkey" },
			{ "TM", "Turkmenistan" },
			{ "TC", "Turks and Caicos Islands" },
			{ "TV", "Tuvalu" },
			{ "UG", "Uganda" },
			{ "UA", "Ukraine" },
			{ "AE", "United Arab Emirates" },
			{ "UK", "United Kingdom" },
			{ "US", "United States" },
			{ "UM", "US Minor Outlying Islands" },
			{ "UY", "Uruguay" },
			{ "UZ", "Uzbekistan" },
			{ "VU", "Vanuatu" },
			{ "VA", "Vatican City State (Holy See)" },
			{ "VE", "Venezuela" },
			{ "VN", "Viet Nam" },
			{ "VG", "Virgin Islands (British)" },
			{ "VI", "Virgin Islands (US)" },
			{ "WF", "Wallis and Futuna Islands" },
			{ "EH", "Western Sahara" },
			{ "YE", "Yemen" },
			{ "YU", "Yugoslavia" },
			{ "ZR", "Zaire" },
			{ "ZM", "Zambia" },
			{ "ZW", "Zimbabwe" }
			#endregion
		};

		/// <summary>
		/// A dictionary of state names <i>keyed by the country abbreviation</i>, with the full state name as the value.
		/// So this allows one to lookup the abbreviation for a state when they already have the full state name.
		/// </summary>
		public static readonly IDictionary<string, string> USCanadaStatesByAbbreviationDict = new Dictionary<string, string>(75, StringComparer.OrdinalIgnoreCase)
		{
			#region ===== US States =====
			{ "AL", "Alabama" },
			{ "AK", "Alaska" },
			{ "AZ", "Arizona" },
			{ "AR", "Arkansas" },
			{ "CA", "California" },
			{ "CO", "Colorado" },
			{ "CT", "Connecticut" },
			{ "DE", "Delaware" },
			{ "FL", "Florida" },
			{ "GA", "Georgia" },
			{ "HI", "Hawaii" },
			{ "ID", "Idaho" },
			{ "IL", "Illinois" },
			{ "IN", "Indiana" },
			{ "IA", "Iowa" },
			{ "KS", "Kansas" },
			{ "KY", "Kentucky" },
			{ "LA", "Louisiana" },
			{ "ME", "Maine" },
			{ "MD", "Maryland" },
			{ "MA", "Massachusetts" },
			{ "MI", "Michigan" },
			{ "MN", "Minnesota" },
			{ "MS", "Mississippi" },
			{ "MO", "Missouri" },
			{ "MT", "Montana" },
			{ "NE", "Nebraska" },
			{ "NV", "Nevada" },
			{ "NH", "New Hampshire" },
			{ "NJ", "New Jersey" },
			{ "NM", "New Mexico" },
			{ "NY", "New York" },
			{ "NC", "North Carolina" },
			{ "ND", "North Dakota" },
			{ "OH", "Ohio" },
			{ "OK", "Oklahoma" },
			{ "OR", "Oregon" },
			{ "PA", "Pennsylvania" },
			{ "RI", "Rhode Island" },
			{ "SC", "South Carolina" },
			{ "SD", "South Dakota" },
			{ "TN", "Tennessee" },
			{ "TX", "Texas" },
			{ "UT", "Utah" },
			{ "VT", "Vermont" },
			{ "VA", "Virginia" },
			{ "WA", "Washington" },
			{ "DC", "Washington DC" },
			{ "WV", "West Virginia" },
			{ "WI", "Wisconsin" },
			{ "WY", "Wyoming" },
			#endregion

			#region ===== Canada States =====

			{ "AB", "Alberta" },
			{ "BC", "British Columbia" },
			{ "MB", "Manitoba" },
			{ "NB", "New Brunswick" },
			{ "NL", "Newfoundland" },
			{ "NT", "Northwest Territories" },
			{ "NS", "Nova Scotia" },
			{ "NU", "Nunavut" },
			{ "ON", "Ontario" },
			{ "PE", "Prince Edward Island" },
			{ "QC", "Quebec" },
			{ "SK", "Saskatchewan" },
			{ "YT", "Yukon" }
			#endregion
		};


		/// <summary>
		/// Set the CountriesTopped field with the input topCountries on top.
		/// Value gotten from GetCountriesWithTheseOnTop.
		/// </summary>
		/// <param name="topCountries">The names of countries (full names or abbreviations both work, even mixed) 
		/// you would like to be set on top in the CountriesTopped field.</param>
		public static void SetCountriesTopped(string[] topCountries)
		{
			GeoCountriesTopped = GetCountriesWithTheseOnTop("");
		}

		public static string[] GetCountriesWithTheseOnTop(params string[] topCountries)
		{
			string[] top = topCountries;
			if(!top.IsNulle()) {
				for(int i = 0; i < top.Length; i++) {
					string nm = top[i];
					if(!nm.IsNulle()) {
						top[i] = nm = nm.Length == 2
							? GeoCountriesByAbbreviationDict.ValueOrDefault(nm)
							: GeoCountriesDictByFullName.ValueOrDefault(nm);
					}
					if(nm.IsNulle())
						top[i] = null;
				}
				top = top.Where(n => n.NotNulle()).ToArray();
			}
			if(top.IsNulle())
				return GeoCountriesByAbbreviationDict.Values.ToArray();

			//bool isAbbrevs = false;

			int cnt = GeoCountriesByAbbreviationDict.Count;
			var d = new Dictionary<string, bool>(cnt);
			List<string> names = new List<string>(cnt);

			// top might have duplicates
			foreach(string name in top) {
				if(!d.ContainsKey(name)) {
					names.Add(name);
					d.Add(name, false);
				}
			}

			// now add items from countries dict only if the value hasn't already been encountered
			foreach(string name in GeoCountriesByAbbreviationDict.Values) {
				if(!d.ContainsKey(name)) {
					names.Add(name);
					d.Add(name, false);
				}
			}

			return names.ToArray();
		}

		#region Country and State Enum Conversions

		static bool _init;

		public static string Name(this USCanadaState value)
		{
			return XEnum<USCanadaState>.Name((int)value);
		}

		public static string Name(this GeoCountry value)
		{
			return XEnum<GeoCountry>.Name((int)value);
		}

		public static string Abbreviation(this USCanadaState value)
		{
			string name = value.Name();
			string abbrv;
			return name.NotNulle() && USCanadaStatesDictByFullName.TryGetValue(name, out abbrv)
				? abbrv : null;
		}

		public static string Abbreviation(this GeoCountry value)
		{
			string name = value.Name();
			string abbrv;
			return name.NotNulle() && GeoCountriesDictByFullName.TryGetValue(name, out abbrv)
				? abbrv : null;
		}

		public static string Name(this USCanadaState? value)
		{
			if(value == null) return null;
			return ((USCanadaState)value).Name();
		}

		public static string Name(this GeoCountry? value)
		{
			if(value == null) return null;
			return ((GeoCountry)value).Name();
		}

		public static string Abbreviation(this USCanadaState? value)
		{
			if(value == null) return null;
			return ((USCanadaState)value).Abbreviation();
		}

		public static string Abbreviation(this GeoCountry? value)
		{
			if(value == null) return null;
			return ((GeoCountry)value).Abbreviation();
		}


		private static void InitUSCanadaStateEnum(ref IDictionary<string, USCanadaState> allStateNamesDict)
		{
			// auto-generated
			var _d = new Dictionary<USCanadaState, string>(65) {
				{ USCanadaState.None, "None" },
				{ USCanadaState.Alabama, "Alabama" },
				{ USCanadaState.Alaska, "Alaska" },
				{ USCanadaState.Alberta, "Alberta" },
				{ USCanadaState.Arizona, "Arizona" },
				{ USCanadaState.Arkansas, "Arkansas" },
				{ USCanadaState.British_Columbia, "British Columbia" },
				{ USCanadaState.California, "California" },
				{ USCanadaState.Colorado, "Colorado" },
				{ USCanadaState.Connecticut, "Connecticut" },
				{ USCanadaState.Delaware, "Delaware" },
				{ USCanadaState.Florida, "Florida" },
				{ USCanadaState.Georgia, "Georgia" },
				{ USCanadaState.Hawaii, "Hawaii" },
				{ USCanadaState.Idaho, "Idaho" },
				{ USCanadaState.Illinois, "Illinois" },
				{ USCanadaState.Indiana, "Indiana" },
				{ USCanadaState.Iowa, "Iowa" },
				{ USCanadaState.Kansas, "Kansas" },
				{ USCanadaState.Kentucky, "Kentucky" },
				{ USCanadaState.Louisiana, "Louisiana" },
				{ USCanadaState.Maine, "Maine" },
				{ USCanadaState.Manitoba, "Manitoba" },
				{ USCanadaState.Maryland, "Maryland" },
				{ USCanadaState.Massachusetts, "Massachusetts" },
				{ USCanadaState.Michigan, "Michigan" },
				{ USCanadaState.Minnesota, "Minnesota" },
				{ USCanadaState.Mississippi, "Mississippi" },
				{ USCanadaState.Missouri, "Missouri" },
				{ USCanadaState.Montana, "Montana" },
				{ USCanadaState.Nebraska, "Nebraska" },
				{ USCanadaState.Nevada, "Nevada" },
				{ USCanadaState.New_Brunswick, "New Brunswick" },
				{ USCanadaState.New_Hampshire, "New Hampshire" },
				{ USCanadaState.New_Jersey, "New Jersey" },
				{ USCanadaState.New_Mexico, "New Mexico" },
				{ USCanadaState.New_York, "New York" },
				{ USCanadaState.Newfoundland, "Newfoundland" },
				{ USCanadaState.North_Carolina, "North Carolina" },
				{ USCanadaState.North_Dakota, "North Dakota" },
				{ USCanadaState.Northwest_Territories, "Northwest Territories" },
				{ USCanadaState.Nova_Scotia, "Nova Scotia" },
				{ USCanadaState.Nunavut, "Nunavut" },
				{ USCanadaState.Ohio, "Ohio" },
				{ USCanadaState.Oklahoma, "Oklahoma" },
				{ USCanadaState.Ontario, "Ontario" },
				{ USCanadaState.Oregon, "Oregon" },
				{ USCanadaState.Pennsylvania, "Pennsylvania" },
				{ USCanadaState.Prince_Edward_Island, "Prince Edward Island" },
				{ USCanadaState.Quebec, "Quebec" },
				{ USCanadaState.Rhode_Island, "Rhode Island" },
				{ USCanadaState.Saskatchewan, "Saskatchewan" },
				{ USCanadaState.South_Carolina, "South Carolina" },
				{ USCanadaState.South_Dakota, "South Dakota" },
				{ USCanadaState.Tennessee, "Tennessee" },
				{ USCanadaState.Texas, "Texas" },
				{ USCanadaState.Utah, "Utah" },
				{ USCanadaState.Vermont, "Vermont" },
				{ USCanadaState.Virginia, "Virginia" },
				{ USCanadaState.Washington, "Washington" },
				{ USCanadaState.Washington_DC, "Washington DC" },
				{ USCanadaState.West_Virginia, "West Virginia" },
				{ USCanadaState.Wisconsin, "Wisconsin" },
				{ USCanadaState.Wyoming, "Wyoming" },
				{ USCanadaState.Yukon, "Yukon" }
			};

			XEnum<USCanadaState>.SetNames(_d);
			XEnum<USCanadaState>.CaseInsensitive = true;

			// ---

			var alld = allStateNamesDict;

			foreach(var kv in XEnum<USCanadaState>.ValuesDict) {

				int intVal = kv.Key;
				string flNm = kv.Value;
				USCanadaState gcVal = (USCanadaState)intVal;
				string enumNm = gcVal.ToString();
				string enumNm_NoUnderscores = enumNm.Replace("_", "");

				if(gcVal == USCanadaState.None)
					continue;

				// Add full country name
				if(!alld.ContainsKey(flNm))
					alld.Add(flNm, gcVal);

				// Add raw enum country name (underscores)
				if(enumNm != flNm && !alld.ContainsKey(enumNm))
					alld.Add(enumNm, gcVal);

				// Add enum country name removed underscores
				if(enumNm_NoUnderscores != enumNm && !alld.ContainsKey(enumNm_NoUnderscores))
					alld.Add(enumNm_NoUnderscores, gcVal);
			}

			// Add abbreviations

			foreach(var kv in USCanadaStatesByAbbreviationDict) {
				string abbr = kv.Key;
				string fnm = kv.Value;
				USCanadaState val = XEnum<USCanadaState>.Value(fnm);

				if(!alld.ContainsKey(abbr))
					alld.Add(abbr, val);
			}
		}

		private static void InitCountryEnum(ref IDictionary<string, GeoCountry> allCountryNamesDict)
		{
			var _d = new Dictionary<GeoCountry, string>(240) {
				{ GeoCountry.None, "None" },
				{ GeoCountry.Afghanistan, "Afghanistan" },
				{ GeoCountry.Albania, "Albania" },
				{ GeoCountry.Algeria, "Algeria" },
				{ GeoCountry.American_Samoa, "American Samoa" },
				{ GeoCountry.Andorra, "Andorra" },
				{ GeoCountry.Angola, "Angola" },
				{ GeoCountry.Anguilla, "Anguilla" },
				{ GeoCountry.Antarctica, "Antarctica" },
				{ GeoCountry.Antigua_and_Barbuda, "Antigua and Barbuda" },
				{ GeoCountry.Argentina, "Argentina" },
				{ GeoCountry.Armenia, "Armenia" },
				{ GeoCountry.Aruba, "Aruba" },
				{ GeoCountry.Australia, "Australia" },
				{ GeoCountry.Austria, "Austria" },
				{ GeoCountry.Azerbaijan, "Azerbaijan" },
				{ GeoCountry.Bahamas, "Bahamas" },
				{ GeoCountry.Bahrain, "Bahrain" },
				{ GeoCountry.Bangladesh, "Bangladesh" },
				{ GeoCountry.Barbados, "Barbados" },
				{ GeoCountry.Belarus, "Belarus" },
				{ GeoCountry.Belgium, "Belgium" },
				{ GeoCountry.Belize, "Belize" },
				{ GeoCountry.Benin, "Benin" },
				{ GeoCountry.Bermuda, "Bermuda" },
				{ GeoCountry.Bhutan, "Bhutan" },
				{ GeoCountry.Bolivia, "Bolivia" },
				{ GeoCountry.Bosnia_and_Herzegovina, "Bosnia and Herzegovina" },
				{ GeoCountry.Botswana, "Botswana" },
				{ GeoCountry.Bouvet_Island, "Bouvet Island" },
				{ GeoCountry.Brazil, "Brazil" },
				{ GeoCountry.British_Indian_Ocean_Territory, "British Indian Ocean Territory" },
				{ GeoCountry.Brunei_Darussalam, "Brunei Darussalam" },
				{ GeoCountry.Bulgaria, "Bulgaria" },
				{ GeoCountry.Burkina_Faso, "Burkina Faso" },
				{ GeoCountry.Burundi, "Burundi" },
				{ GeoCountry.Cambodia, "Cambodia" },
				{ GeoCountry.Cameroon, "Cameroon" },
				{ GeoCountry.Canada, "Canada" },
				{ GeoCountry.Cape_Verde, "Cape Verde" },
				{ GeoCountry.Cayman_Islands, "Cayman Islands" },
				{ GeoCountry.Central_African_Republic, "Central African Republic" },
				{ GeoCountry.Chad, "Chad" },
				{ GeoCountry.Chile, "Chile" },
				{ GeoCountry.China, "China" },
				{ GeoCountry.Christmas_Island, "Christmas Island" },
				{ GeoCountry.Cocos_Keeling_Islands, "Cocos (Keeling Islands)" },
				{ GeoCountry.Colombia, "Colombia" },
				{ GeoCountry.Comoros, "Comoros" },
				{ GeoCountry.Congo, "Congo" },
				{ GeoCountry.Cook_Islands, "Cook Islands" },
				{ GeoCountry.Costa_Rica, "Costa Rica" },
				{ GeoCountry.Cote_DIvoire_Ivory_Coast, "Cote D'Ivoire (Ivory Coast)" },
				{ GeoCountry.Croatia_Hrvatska, "Croatia (Hrvatska)" },
				{ GeoCountry.Cuba, "Cuba" },
				{ GeoCountry.Cyprus, "Cyprus" },
				{ GeoCountry.Czech_Republic, "Czech Republic" },
				{ GeoCountry.Denmark, "Denmark" },
				{ GeoCountry.Djibouti, "Djibouti" },
				{ GeoCountry.Dominica, "Dominica" },
				{ GeoCountry.Dominican_Republic, "Dominican Republic" },
				{ GeoCountry.East_Timor, "East Timor" },
				{ GeoCountry.Ecuador, "Ecuador" },
				{ GeoCountry.Egypt, "Egypt" },
				{ GeoCountry.El_Salvador, "El Salvador" },
				{ GeoCountry.Equatorial_Guinea, "Equatorial Guinea" },
				{ GeoCountry.Eritrea, "Eritrea" },
				{ GeoCountry.Estonia, "Estonia" },
				{ GeoCountry.Ethiopia, "Ethiopia" },
				{ GeoCountry.Falkland_Islands_Malvinas, "Falkland Islands (Malvinas)" },
				{ GeoCountry.Faroe_Islands, "Faroe Islands" },
				{ GeoCountry.Fiji, "Fiji" },
				{ GeoCountry.Finland, "Finland" },
				{ GeoCountry.France, "France" },
				{ GeoCountry.France_Metropolitan, "France, Metropolitan" },
				{ GeoCountry.French_Guiana, "French Guiana" },
				{ GeoCountry.French_Polynesia, "French Polynesia" },
				{ GeoCountry.French_Southern_Territories, "French Southern Territories" },
				{ GeoCountry.Gabon, "Gabon" },
				{ GeoCountry.Gambia, "Gambia" },
				{ GeoCountry.Georgia, "Georgia" },
				{ GeoCountry.Germany, "Germany" },
				{ GeoCountry.Ghana, "Ghana" },
				{ GeoCountry.Gibraltar, "Gibraltar" },
				{ GeoCountry.Greece, "Greece" },
				{ GeoCountry.Greenland, "Greenland" },
				{ GeoCountry.Grenada, "Grenada" },
				{ GeoCountry.Guadeloupe, "Guadeloupe" },
				{ GeoCountry.Guam, "Guam" },
				{ GeoCountry.Guatemala, "Guatemala" },
				{ GeoCountry.Guinea, "Guinea" },
				{ GeoCountry.GuineaBissau, "Guinea-Bissau" },
				{ GeoCountry.Guyana, "Guyana" },
				{ GeoCountry.Haiti, "Haiti" },
				{ GeoCountry.Heard_and_McDonald_Islands, "Heard and McDonald Islands" },
				{ GeoCountry.Honduras, "Honduras" },
				{ GeoCountry.Hong_Kong, "Hong Kong" },
				{ GeoCountry.Hungary, "Hungary" },
				{ GeoCountry.Iceland, "Iceland" },
				{ GeoCountry.India, "India" },
				{ GeoCountry.Indonesia, "Indonesia" },
				{ GeoCountry.Iran, "Iran" },
				{ GeoCountry.Iraq, "Iraq" },
				{ GeoCountry.Ireland, "Ireland" },
				{ GeoCountry.Israel, "Israel" },
				{ GeoCountry.Italy, "Italy" },
				{ GeoCountry.Jamaica, "Jamaica" },
				{ GeoCountry.Japan, "Japan" },
				{ GeoCountry.Jordan, "Jordan" },
				{ GeoCountry.Kazakhstan, "Kazakhstan" },
				{ GeoCountry.Kenya, "Kenya" },
				{ GeoCountry.Kiribati, "Kiribati" },
				{ GeoCountry.Kuwait, "Kuwait" },
				{ GeoCountry.Kyrgyzstan, "Kyrgyzstan" },
				{ GeoCountry.Laos, "Laos" },
				{ GeoCountry.Latvia, "Latvia" },
				{ GeoCountry.Lebanon, "Lebanon" },
				{ GeoCountry.Lesotho, "Lesotho" },
				{ GeoCountry.Liberia, "Liberia" },
				{ GeoCountry.Libya, "Libya" },
				{ GeoCountry.Liechtenstein, "Liechtenstein" },
				{ GeoCountry.Lithuania, "Lithuania" },
				{ GeoCountry.Luxembourg, "Luxembourg" },
				{ GeoCountry.Macau, "Macau" },
				{ GeoCountry.Macedonia, "Macedonia" },
				{ GeoCountry.Madagascar, "Madagascar" },
				{ GeoCountry.Malawi, "Malawi" },
				{ GeoCountry.Malaysia, "Malaysia" },
				{ GeoCountry.Maldives, "Maldives" },
				{ GeoCountry.Mali, "Mali" },
				{ GeoCountry.Malta, "Malta" },
				{ GeoCountry.Marshall_Islands, "Marshall Islands" },
				{ GeoCountry.Martinique, "Martinique" },
				{ GeoCountry.Mauritania, "Mauritania" },
				{ GeoCountry.Mauritius, "Mauritius" },
				{ GeoCountry.Mayotte, "Mayotte" },
				{ GeoCountry.Mexico, "Mexico" },
				{ GeoCountry.Micronesia, "Micronesia" },
				{ GeoCountry.Moldova, "Moldova" },
				{ GeoCountry.Monaco, "Monaco" },
				{ GeoCountry.Mongolia, "Mongolia" },
				{ GeoCountry.Montserrat, "Montserrat" },
				{ GeoCountry.Morocco, "Morocco" },
				{ GeoCountry.Mozambique, "Mozambique" },
				{ GeoCountry.Myanmar, "Myanmar" },
				{ GeoCountry.Namibia, "Namibia" },
				{ GeoCountry.Nauru, "Nauru" },
				{ GeoCountry.Nepal, "Nepal" },
				{ GeoCountry.Netherlands, "Netherlands" },
				{ GeoCountry.Netherlands_Antilles, "Netherlands Antilles" },
				{ GeoCountry.New_Caledonia, "New Caledonia" },
				{ GeoCountry.New_Zealand, "New Zealand" },
				{ GeoCountry.Nicaragua, "Nicaragua" },
				{ GeoCountry.Niger, "Niger" },
				{ GeoCountry.Nigeria, "Nigeria" },
				{ GeoCountry.Niue, "Niue" },
				{ GeoCountry.Norfolk_Island, "Norfolk Island" },
				{ GeoCountry.North_Korea, "North Korea" },
				{ GeoCountry.Northern_Mariana_Islands, "Northern Mariana Islands" },
				{ GeoCountry.Norway, "Norway" },
				{ GeoCountry.Oman, "Oman" },
				{ GeoCountry.Pakistan, "Pakistan" },
				{ GeoCountry.Palau, "Palau" },
				{ GeoCountry.Panama, "Panama" },
				{ GeoCountry.Papua_New_Guinea, "Papua New Guinea" },
				{ GeoCountry.Paraguay, "Paraguay" },
				{ GeoCountry.Peru, "Peru" },
				{ GeoCountry.Philippines, "Philippines" },
				{ GeoCountry.Pitcairn, "Pitcairn" },
				{ GeoCountry.Poland, "Poland" },
				{ GeoCountry.Portugal, "Portugal" },
				{ GeoCountry.Puerto_Rico, "Puerto Rico" },
				{ GeoCountry.Qatar, "Qatar" },
				{ GeoCountry.Reunion, "Reunion" },
				{ GeoCountry.Romania, "Romania" },
				{ GeoCountry.Russian_Federation, "Russian Federation" },
				{ GeoCountry.Rwanda, "Rwanda" },
				{ GeoCountry.S_Georgia_and_S_Sandwich_Isls, "S. Georgia and S. Sandwich Isls." },
				{ GeoCountry.Saint_Kitts_and_Nevis, "Saint Kitts and Nevis" },
				{ GeoCountry.Saint_Lucia, "Saint Lucia" },
				{ GeoCountry.Saint_Vincent_and_The_Grenadines, "Saint Vincent and The Grenadines" },
				{ GeoCountry.Samoa, "Samoa" },
				{ GeoCountry.San_Marino, "San Marino" },
				{ GeoCountry.Sao_Tome_and_Principe, "Sao Tome and Principe" },
				{ GeoCountry.Saudi_Arabia, "Saudi Arabia" },
				{ GeoCountry.Senegal, "Senegal" },
				{ GeoCountry.Seychelles, "Seychelles" },
				{ GeoCountry.Sierra_Leone, "Sierra Leone" },
				{ GeoCountry.Singapore, "Singapore" },
				{ GeoCountry.Slovak_Republic, "Slovak Republic" },
				{ GeoCountry.Slovenia, "Slovenia" },
				{ GeoCountry.Solomon_Islands, "Solomon Islands" },
				{ GeoCountry.Somalia, "Somalia" },
				{ GeoCountry.Sourth_Korea, "Sourth Korea" },
				{ GeoCountry.South_Africa, "South Africa" },
				{ GeoCountry.Spain, "Spain" },
				{ GeoCountry.Sri_Lanka, "Sri Lanka" },
				{ GeoCountry.St_Helena, "St. Helena" },
				{ GeoCountry.St_Pierre_and_Miquelon, "St. Pierre and Miquelon" },
				{ GeoCountry.Sudan, "Sudan" },
				{ GeoCountry.Suriname, "Suriname" },
				{ GeoCountry.Svalbard_and_Jan_Mayen_Islands, "Svalbard and Jan Mayen Islands" },
				{ GeoCountry.Swaziland, "Swaziland" },
				{ GeoCountry.Sweden, "Sweden" },
				{ GeoCountry.Switzerland, "Switzerland" },
				{ GeoCountry.Syria, "Syria" },
				{ GeoCountry.Taiwan, "Taiwan" },
				{ GeoCountry.Tajikistan, "Tajikistan" },
				{ GeoCountry.Tanzania, "Tanzania" },
				{ GeoCountry.Thailand, "Thailand" },
				{ GeoCountry.Togo, "Togo" },
				{ GeoCountry.Tokelau, "Tokelau" },
				{ GeoCountry.Tonga, "Tonga" },
				{ GeoCountry.Trinidad_and_Tobago, "Trinidad and Tobago" },
				{ GeoCountry.Tunisia, "Tunisia" },
				{ GeoCountry.Turkey, "Turkey" },
				{ GeoCountry.Turkmenistan, "Turkmenistan" },
				{ GeoCountry.Turks_and_Caicos_Islands, "Turks and Caicos Islands" },
				{ GeoCountry.Tuvalu, "Tuvalu" },
				{ GeoCountry.Uganda, "Uganda" },
				{ GeoCountry.Ukraine, "Ukraine" },
				{ GeoCountry.United_Arab_Emirates, "United Arab Emirates" },
				{ GeoCountry.United_Kingdom, "United Kingdom" },
				{ GeoCountry.United_States, "United States" },
				{ GeoCountry.Uruguay, "Uruguay" },
				{ GeoCountry.US_Minor_Outlying_Islands, "US Minor Outlying Islands" },
				{ GeoCountry.Uzbekistan, "Uzbekistan" },
				{ GeoCountry.Vanuatu, "Vanuatu" },
				{ GeoCountry.Vatican_City_State_Holy_See, "Vatican City State (Holy See)" },
				{ GeoCountry.Venezuela, "Venezuela" },
				{ GeoCountry.Viet_Nam, "Viet Nam" },
				{ GeoCountry.Virgin_Islands_British, "Virgin Islands (British)" },
				{ GeoCountry.Virgin_Islands_US, "Virgin Islands (US)" },
				{ GeoCountry.Wallis_and_Futuna_Islands, "Wallis and Futuna Islands" },
				{ GeoCountry.Western_Sahara, "Western Sahara" },
				{ GeoCountry.Yemen, "Yemen" },
				{ GeoCountry.Yugoslavia, "Yugoslavia" },
				{ GeoCountry.Zaire, "Zaire" },
				{ GeoCountry.Zambia, "Zambia" },
				{ GeoCountry.Zimbabwe, "Zimbabwe" }
			};

			XEnum<GeoCountry>.SetNames(_d);
			XEnum<GeoCountry>.CaseInsensitive = true;

			// ---

			var alld = allCountryNamesDict;

			foreach(var kv in XEnum<GeoCountry>.ValuesDict) {

				int intVal = kv.Key;
				string flNm = kv.Value;
				GeoCountry gcVal = (GeoCountry)intVal;
				string enumNm = gcVal.ToString();
				string enumNm_NoUnderscores = enumNm.Replace("_", "");

				if(gcVal == GeoCountry.None)
					continue;

				// Add full country name
				if(!alld.ContainsKey(flNm))
					alld.Add(flNm, gcVal);

				// Add raw enum country name (underscores)
				if(enumNm != flNm && !alld.ContainsKey(enumNm))
					alld.Add(enumNm, gcVal);

				// Add enum country name removed underscores
				if(enumNm_NoUnderscores != enumNm && !alld.ContainsKey(enumNm_NoUnderscores))
					alld.Add(enumNm_NoUnderscores, gcVal);
			}

			// Add abbreviations

			foreach(var kv in GeoCountriesByAbbreviationDict) {
				string abbr = kv.Key;
				string fnm = kv.Value;
				GeoCountry val = XEnum<GeoCountry>.Value(fnm);

				if(!alld.ContainsKey(abbr))
					alld.Add(abbr, val);
			}
		}

		public static readonly IDictionary<string, GeoCountry> AllCountryNamesDict;
		public static readonly IDictionary<string, USCanadaState> AllUSCanadaStateNamesDict;

		#endregion

		#region Code Generator

		/// <summary>
		/// Gets C# code generated from the current Countries and States 
		/// dictionaries for making them into enums.
		/// </summary>
		/// <returns></returns>
		public static string GetGeoNamesEnumsCode()
		{
			string[] _countries = GeoCountries.Concat("None", itemFirst: true).ToArray();
			string countryEnums = XEnum.GetEnumCode("GeoCountry", _countries);

			string[] _states = USCanadaStates.Concat("None", itemFirst: true).ToArray();
			string stateEnums = XEnum.GetEnumCode("USCanadaState", _states);

			string all = "\r\n" + countryEnums + "\r\n" + stateEnums + "\r\n";
			return all;
		}

		#endregion




		/// <summary>
		/// Returns the Country enum value for input string of a country 
		/// full name, or two-letter abbreviation, or string representation of the enum 
		/// (with and without underscores). Throws if not found.
		/// </summary>
		public static GeoCountry GetCountry(string value)
			=> AllCountryNamesDict[value];

		/// <summary>
		/// See notes on <see cref="GetCountry(string)"/>, this returns null if not found.
		/// </summary>
		public static GeoCountry? GetCountryOrNull(string value)
			=> AllCountryNamesDict.ValueN(value);

		/// <summary>
		/// Returns the State enum value for input string of a state 
		/// full name, or two-letter abbreviation, or string representation of the enum 
		/// (with and without underscores). Throws if not found.
		/// </summary>
		public static USCanadaState GetState(string value)
			=> AllUSCanadaStateNamesDict[value];

		/// <summary>
		/// See notes on <see cref="GetState(string)"/>, this returns null if not found.
		/// </summary>
		public static USCanadaState? GetStateOrNull(string value)
			=> AllUSCanadaStateNamesDict.ValueN(value);
	}
}
