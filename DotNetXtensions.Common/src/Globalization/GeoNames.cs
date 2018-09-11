using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DotNetXtensions.Globalization
{
	/// <summary>
	/// Class containing geographical and national information, currently world country names and state names.
	/// </summary>
	public static class GeoNames
	{
		// howdy?!
		/// <summary>
		/// Country names.
		/// </summary>
		public static readonly string[] Countries;

		/// <summary>
		/// US and Canadian state names.
		/// </summary>
		public static readonly string[] States;

		/// <summary>
		/// Country names sorted. This is useful when conducting a binary search.
		/// </summary>
		public static readonly string[] CountriesOrdered;

		/// <summary>
		/// US and Canadian state names ordered. This is useful when conducting a binary search.
		/// </summary>
		public static readonly string[] StatesOrdered;

		/// <summary>
		/// Country names with a few of the common countries many of us have to deal with on top.
		/// You can change these values (or simply never use this!) by calling SetCountriesTopped.
		/// </summary>
		public static string[] CountriesTopped;

		/// <summary>
		/// A dictionary of country names <i>keyed by the full country name</i>, with the abbreviation as the value.
		/// So this allows one to lookup the abbreviation for a country when they already have the full country name.
		/// </summary>
		public static readonly IDictionary<string, string> CountriesDictByFullName;

		/// <summary>
		/// A dictionary of state names <i>keyed by the full state name</i>, with the abbreviation as the value.
		/// So this allows one to lookup the abbreviation for a state when they already have the full state name.
		/// </summary>
		public static readonly IDictionary<string, string> StatesDictByFullName;



		// STATIC CONSTRUCTOR

		static GeoNames()
		{
			Countries = CountriesDict.Values.ToArray().Sort();
			States = StatesDict.Values.ToArray().Sort();

			CountriesOrdered = Countries.OrderBy(s => s).ToArray();
			StatesOrdered = States.OrderBy(s => s).ToArray();

			CountriesDictByFullName = CountriesDict.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);
			StatesDictByFullName = StatesDict.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

			// important, init this after these others (at least the country inits)
			CountriesTopped = GetCountriesWithTheseOnTop("US", "CA", "UK", "DE", "AU");
		}




		// ===== ALL THE INFORMATION COMES FROM THESE TWO DICTIONARIES ======

		/// <summary>
		/// A dictionary of country names <i>keyed by the country abbreviation</i>, with the full country name as the value.
		/// So this allows one to lookup the abbreviation for a country when they already have the full name.
		/// <remarks>
		/// Information was initially based on: http://www.paladinsoftware.com/Generic/countries.htm (Nov 2013)
		/// </remarks>
		/// </summary>
		public static readonly IDictionary<string, string> CountriesDict = new Dictionary<string, string>(250, StringComparer.OrdinalIgnoreCase)
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
		public static readonly IDictionary<string, string> StatesDict = new Dictionary<string, string>(75, StringComparer.OrdinalIgnoreCase)
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
			CountriesTopped = GetCountriesWithTheseOnTop("");
		}

		public static string[] GetCountriesWithTheseOnTop(params string[] topCountries)
		{
			string[] top = topCountries;
			if (!top.IsNulle()) {
				for (int i = 0; i < top.Length; i++) {
					string nm = top[i];
					if (!nm.IsNulle()) {
						top[i] = nm = nm.Length == 2
							? CountriesDict.ValueOrDefault(nm)
							: CountriesDictByFullName.ValueOrDefault(nm);
					}
					if (nm.IsNulle())
						top[i] = null;
				}
				top = top.Where(n => n.NotNulle()).ToArray();
			}
			if (top.IsNulle())
				return CountriesDict.Values.ToArray();

			//bool isAbbrevs = false;

			int cnt = CountriesDict.Count;
			var d = new Dictionary<string, bool>(cnt);
			List<string> names = new List<string>(cnt);

			// top might have duplicates
			foreach (string name in top) {
				if (!d.ContainsKey(name)) {
					names.Add(name);
					d.Add(name, false);
				}
			}

			// now add items from countries dict only if the value hasn't already been encountered
			foreach (string name in CountriesDict.Values) {
				if (!d.ContainsKey(name)) {
					names.Add(name);
					d.Add(name, false);
				}
			}

			return names.ToArray();
		}

		#region Country and State Enum Conversions

		static bool _init;

		public static string Name(this State value)
		{
			Init();
			return XEnum<State>.Name((int)value);
		}

		public static string Name(this Country value)
		{
			Init();
			return XEnum<Country>.Name((int)value);
		}

		public static string Abbreviation(this State value)
		{
			string name = value.Name();
			string abbrv;
			return name.NotNulle() && StatesDictByFullName.TryGetValue(name, out abbrv)
				? abbrv : null;
		}

		public static string Abbreviation(this Country value)
		{
			string name = value.Name();
			string abbrv;
			return name.NotNulle() && CountriesDictByFullName.TryGetValue(name, out abbrv)
				? abbrv : null;
		}

		public static string Name(this State? value)
		{
			if (value == null) return null;
			return ((State)value).Name();
		}

		public static string Name(this Country? value)
		{
			if (value == null) return null;
			return ((Country)value).Name();
		}

		public static string Abbreviation(this State? value)
		{
			if (value == null) return null;
			return ((State)value).Abbreviation();
		}

		public static string Abbreviation(this Country? value)
		{
			if (value == null) return null;
			return ((Country)value).Abbreviation();
		}

		public static string GetLongCountryName(string name)
		{
			if (name != null && name.Length == 2) {
				string lnm;
				if (CountriesDict.TryGetValue(name, out lnm))
					return lnm;
			}
			return name;
		}

		public static string GetLongStateName(string name)
		{
			if (name != null && name.Length == 2) {
				string lnm;
				if (StatesDict.TryGetValue(name, out lnm))
					return lnm;
			}
			return name;
		}

		public static Country ToCountry(string value)
		{
			Init();
			return XEnum<Country>.Value(GetLongCountryName(value));
		}

		public static State ToState(string value)
		{
			Init();
			return XEnum<State>.Value(GetLongStateName(value));
		}

		public static Country? ToCountryOrNull(string value)
		{
			Init();
			if (!value.IsNulle()) {
				Country val;
				if (XEnum<Country>.TryGetValue(GetLongCountryName(value), out val))
					return val;
			}
			return null;
		}

		public static State? ToStateOrNull(string value)
		{
			Init();
			if (!value.IsNulle()) {
				State val;
				if (XEnum<State>.TryGetValue(GetLongStateName(value), out val))
					return val;
			}
			return null;
		}

		private static void Init()
		{
			if (!_init) {
				_init = true;
				InitStateEnum();
				InitCountryEnum();
			}
		}

		private static void InitStateEnum()
		{
			var _names = new Dictionary<State, string>(64) {
{ State.Alabama, "Alabama" },
{ State.Alaska, "Alaska" },
{ State.Arizona, "Arizona" },
{ State.Arkansas, "Arkansas" },
{ State.California, "California" },
{ State.Colorado, "Colorado" },
{ State.Connecticut, "Connecticut" },
{ State.Delaware, "Delaware" },
{ State.Washington_DC, "Washington DC" },
{ State.Florida, "Florida" },
{ State.Georgia, "Georgia" },
{ State.Hawaii, "Hawaii" },
{ State.Idaho, "Idaho" },
{ State.Illinois, "Illinois" },
{ State.Indiana, "Indiana" },
{ State.Iowa, "Iowa" },
{ State.Kansas, "Kansas" },
{ State.Kentucky, "Kentucky" },
{ State.Louisiana, "Louisiana" },
{ State.Maine, "Maine" },
{ State.Maryland, "Maryland" },
{ State.Massachusetts, "Massachusetts" },
{ State.Michigan, "Michigan" },
{ State.Minnesota, "Minnesota" },
{ State.Mississippi, "Mississippi" },
{ State.Missouri, "Missouri" },
{ State.Montana, "Montana" },
{ State.Nebraska, "Nebraska" },
{ State.Nevada, "Nevada" },
{ State.New_Hampshire, "New Hampshire" },
{ State.New_Jersey, "New Jersey" },
{ State.New_Mexico, "New Mexico" },
{ State.New_York, "New York" },
{ State.North_Carolina, "North Carolina" },
{ State.North_Dakota, "North Dakota" },
{ State.Ohio, "Ohio" },
{ State.Oklahoma, "Oklahoma" },
{ State.Oregon, "Oregon" },
{ State.Pennsylvania, "Pennsylvania" },
{ State.Rhode_Island, "Rhode Island" },
{ State.South_Carolina, "South Carolina" },
{ State.South_Dakota, "South Dakota" },
{ State.Tennessee, "Tennessee" },
{ State.Texas, "Texas" },
{ State.Utah, "Utah" },
{ State.Vermont, "Vermont" },
{ State.Virginia, "Virginia" },
{ State.Washington, "Washington" },
{ State.West_Virginia, "West Virginia" },
{ State.Wisconsin, "Wisconsin" },
{ State.Wyoming, "Wyoming" },
{ State.Alberta, "Alberta" },
{ State.British_Columbia, "British Columbia" },
{ State.Manitoba, "Manitoba" },
{ State.New_Brunswick, "New Brunswick" },
{ State.Newfoundland, "Newfoundland" },
{ State.Northwest_Territories, "Northwest Territories" },
{ State.Nova_Scotia, "Nova Scotia" },
{ State.Nunavut, "Nunavut" },
{ State.Ontario, "Ontario" },
{ State.Prince_Edward_Island, "Prince Edward Island" },
{ State.Quebec, "Quebec" },
{ State.Saskatchewan, "Saskatchewan" },
{ State.Yukon, "Yukon" }
};
			XEnum<State>.SetNames(_names);
			XEnum<State>.CaseInsensitive = true;
		}

		private static void InitCountryEnum()
		{
			var _names = new Dictionary<Country, string>(239) {
{ Country.Afghanistan, "Afghanistan" },
{ Country.Albania, "Albania" },
{ Country.Algeria, "Algeria" },
{ Country.American_Samoa, "American Samoa" },
{ Country.Andorra, "Andorra" },
{ Country.Angola, "Angola" },
{ Country.Anguilla, "Anguilla" },
{ Country.Antarctica, "Antarctica" },
{ Country.Antigua_and_Barbuda, "Antigua and Barbuda" },
{ Country.Argentina, "Argentina" },
{ Country.Armenia, "Armenia" },
{ Country.Aruba, "Aruba" },
{ Country.Australia, "Australia" },
{ Country.Austria, "Austria" },
{ Country.Azerbaijan, "Azerbaijan" },
{ Country.Bahamas, "Bahamas" },
{ Country.Bahrain, "Bahrain" },
{ Country.Bangladesh, "Bangladesh" },
{ Country.Barbados, "Barbados" },
{ Country.Belarus, "Belarus" },
{ Country.Belgium, "Belgium" },
{ Country.Belize, "Belize" },
{ Country.Benin, "Benin" },
{ Country.Bermuda, "Bermuda" },
{ Country.Bhutan, "Bhutan" },
{ Country.Bolivia, "Bolivia" },
{ Country.Bosnia_and_Herzegovina, "Bosnia and Herzegovina" },
{ Country.Botswana, "Botswana" },
{ Country.Bouvet_Island, "Bouvet Island" },
{ Country.Brazil, "Brazil" },
{ Country.British_Indian_Ocean_Territory, "British Indian Ocean Territory" },
{ Country.Brunei_Darussalam, "Brunei Darussalam" },
{ Country.Bulgaria, "Bulgaria" },
{ Country.Burkina_Faso, "Burkina Faso" },
{ Country.Burundi, "Burundi" },
{ Country.Cambodia, "Cambodia" },
{ Country.Cameroon, "Cameroon" },
{ Country.Canada, "Canada" },
{ Country.Cape_Verde, "Cape Verde" },
{ Country.Cayman_Islands, "Cayman Islands" },
{ Country.Central_African_Republic, "Central African Republic" },
{ Country.Chad, "Chad" },
{ Country.Chile, "Chile" },
{ Country.China, "China" },
{ Country.Christmas_Island, "Christmas Island" },
{ Country.Cocos_Keeling_Islands, "Cocos (Keeling Islands)" },
{ Country.Colombia, "Colombia" },
{ Country.Comoros, "Comoros" },
{ Country.Congo, "Congo" },
{ Country.Cook_Islands, "Cook Islands" },
{ Country.Costa_Rica, "Costa Rica" },
{ Country.Cote_DIvoire_Ivory_Coast, "Cote D'Ivoire (Ivory Coast)" },
{ Country.Croatia_Hrvatska, "Croatia (Hrvatska)" },
{ Country.Cuba, "Cuba" },
{ Country.Cyprus, "Cyprus" },
{ Country.Czech_Republic, "Czech Republic" },
{ Country.Denmark, "Denmark" },
{ Country.Djibouti, "Djibouti" },
{ Country.Dominica, "Dominica" },
{ Country.Dominican_Republic, "Dominican Republic" },
{ Country.East_Timor, "East Timor" },
{ Country.Ecuador, "Ecuador" },
{ Country.Egypt, "Egypt" },
{ Country.El_Salvador, "El Salvador" },
{ Country.Equatorial_Guinea, "Equatorial Guinea" },
{ Country.Eritrea, "Eritrea" },
{ Country.Estonia, "Estonia" },
{ Country.Ethiopia, "Ethiopia" },
{ Country.Falkland_Islands_Malvinas, "Falkland Islands (Malvinas)" },
{ Country.Faroe_Islands, "Faroe Islands" },
{ Country.Fiji, "Fiji" },
{ Country.Finland, "Finland" },
{ Country.France, "France" },
{ Country.France_Metropolitan, "France, Metropolitan" },
{ Country.French_Guiana, "French Guiana" },
{ Country.French_Polynesia, "French Polynesia" },
{ Country.French_Southern_Territories, "French Southern Territories" },
{ Country.Gabon, "Gabon" },
{ Country.Gambia, "Gambia" },
{ Country.Georgia, "Georgia" },
{ Country.Germany, "Germany" },
{ Country.Ghana, "Ghana" },
{ Country.Gibraltar, "Gibraltar" },
{ Country.Greece, "Greece" },
{ Country.Greenland, "Greenland" },
{ Country.Grenada, "Grenada" },
{ Country.Guadeloupe, "Guadeloupe" },
{ Country.Guam, "Guam" },
{ Country.Guatemala, "Guatemala" },
{ Country.Guinea, "Guinea" },
{ Country.GuineaBissau, "Guinea-Bissau" },
{ Country.Guyana, "Guyana" },
{ Country.Haiti, "Haiti" },
{ Country.Heard_and_McDonald_Islands, "Heard and McDonald Islands" },
{ Country.Honduras, "Honduras" },
{ Country.Hong_Kong, "Hong Kong" },
{ Country.Hungary, "Hungary" },
{ Country.Iceland, "Iceland" },
{ Country.India, "India" },
{ Country.Indonesia, "Indonesia" },
{ Country.Iran, "Iran" },
{ Country.Iraq, "Iraq" },
{ Country.Ireland, "Ireland" },
{ Country.Israel, "Israel" },
{ Country.Italy, "Italy" },
{ Country.Jamaica, "Jamaica" },
{ Country.Japan, "Japan" },
{ Country.Jordan, "Jordan" },
{ Country.Kazakhstan, "Kazakhstan" },
{ Country.Kenya, "Kenya" },
{ Country.Kiribati, "Kiribati" },
{ Country.Kuwait, "Kuwait" },
{ Country.Kyrgyzstan, "Kyrgyzstan" },
{ Country.Laos, "Laos" },
{ Country.Latvia, "Latvia" },
{ Country.Lebanon, "Lebanon" },
{ Country.Lesotho, "Lesotho" },
{ Country.Liberia, "Liberia" },
{ Country.Libya, "Libya" },
{ Country.Liechtenstein, "Liechtenstein" },
{ Country.Lithuania, "Lithuania" },
{ Country.Luxembourg, "Luxembourg" },
{ Country.Macau, "Macau" },
{ Country.Macedonia, "Macedonia" },
{ Country.Madagascar, "Madagascar" },
{ Country.Malawi, "Malawi" },
{ Country.Malaysia, "Malaysia" },
{ Country.Maldives, "Maldives" },
{ Country.Mali, "Mali" },
{ Country.Malta, "Malta" },
{ Country.Marshall_Islands, "Marshall Islands" },
{ Country.Martinique, "Martinique" },
{ Country.Mauritania, "Mauritania" },
{ Country.Mauritius, "Mauritius" },
{ Country.Mayotte, "Mayotte" },
{ Country.Mexico, "Mexico" },
{ Country.Micronesia, "Micronesia" },
{ Country.Moldova, "Moldova" },
{ Country.Monaco, "Monaco" },
{ Country.Mongolia, "Mongolia" },
{ Country.Montserrat, "Montserrat" },
{ Country.Morocco, "Morocco" },
{ Country.Mozambique, "Mozambique" },
{ Country.Myanmar, "Myanmar" },
{ Country.Namibia, "Namibia" },
{ Country.Nauru, "Nauru" },
{ Country.Nepal, "Nepal" },
{ Country.Netherlands, "Netherlands" },
{ Country.Netherlands_Antilles, "Netherlands Antilles" },
{ Country.New_Caledonia, "New Caledonia" },
{ Country.New_Zealand, "New Zealand" },
{ Country.Nicaragua, "Nicaragua" },
{ Country.Niger, "Niger" },
{ Country.Nigeria, "Nigeria" },
{ Country.Niue, "Niue" },
{ Country.Norfolk_Island, "Norfolk Island" },
{ Country.North_Korea, "North Korea" },
{ Country.Northern_Mariana_Islands, "Northern Mariana Islands" },
{ Country.Norway, "Norway" },
{ Country.Oman, "Oman" },
{ Country.Pakistan, "Pakistan" },
{ Country.Palau, "Palau" },
{ Country.Panama, "Panama" },
{ Country.Papua_New_Guinea, "Papua New Guinea" },
{ Country.Paraguay, "Paraguay" },
{ Country.Peru, "Peru" },
{ Country.Philippines, "Philippines" },
{ Country.Pitcairn, "Pitcairn" },
{ Country.Poland, "Poland" },
{ Country.Portugal, "Portugal" },
{ Country.Puerto_Rico, "Puerto Rico" },
{ Country.Qatar, "Qatar" },
{ Country.Reunion, "Reunion" },
{ Country.Romania, "Romania" },
{ Country.Russian_Federation, "Russian Federation" },
{ Country.Rwanda, "Rwanda" },
{ Country.S_Georgia_and_S_Sandwich_Isls, "S. Georgia and S. Sandwich Isls." },
{ Country.Saint_Kitts_and_Nevis, "Saint Kitts and Nevis" },
{ Country.Saint_Lucia, "Saint Lucia" },
{ Country.Saint_Vincent_and_The_Grenadines, "Saint Vincent and The Grenadines" },
{ Country.Samoa, "Samoa" },
{ Country.San_Marino, "San Marino" },
{ Country.Sao_Tome_and_Principe, "Sao Tome and Principe" },
{ Country.Saudi_Arabia, "Saudi Arabia" },
{ Country.Senegal, "Senegal" },
{ Country.Seychelles, "Seychelles" },
{ Country.Sierra_Leone, "Sierra Leone" },
{ Country.Singapore, "Singapore" },
{ Country.Slovak_Republic, "Slovak Republic" },
{ Country.Slovenia, "Slovenia" },
{ Country.Solomon_Islands, "Solomon Islands" },
{ Country.Somalia, "Somalia" },
{ Country.South_Africa, "South Africa" },
{ Country.Sourth_Korea, "Sourth Korea" },
{ Country.Spain, "Spain" },
{ Country.Sri_Lanka, "Sri Lanka" },
{ Country.St_Helena, "St. Helena" },
{ Country.St_Pierre_and_Miquelon, "St. Pierre and Miquelon" },
{ Country.Sudan, "Sudan" },
{ Country.Suriname, "Suriname" },
{ Country.Svalbard_and_Jan_Mayen_Islands, "Svalbard and Jan Mayen Islands" },
{ Country.Swaziland, "Swaziland" },
{ Country.Sweden, "Sweden" },
{ Country.Switzerland, "Switzerland" },
{ Country.Syria, "Syria" },
{ Country.Taiwan, "Taiwan" },
{ Country.Tajikistan, "Tajikistan" },
{ Country.Tanzania, "Tanzania" },
{ Country.Thailand, "Thailand" },
{ Country.Togo, "Togo" },
{ Country.Tokelau, "Tokelau" },
{ Country.Tonga, "Tonga" },
{ Country.Trinidad_and_Tobago, "Trinidad and Tobago" },
{ Country.Tunisia, "Tunisia" },
{ Country.Turkey, "Turkey" },
{ Country.Turkmenistan, "Turkmenistan" },
{ Country.Turks_and_Caicos_Islands, "Turks and Caicos Islands" },
{ Country.Tuvalu, "Tuvalu" },
{ Country.Uganda, "Uganda" },
{ Country.Ukraine, "Ukraine" },
{ Country.United_Arab_Emirates, "United Arab Emirates" },
{ Country.United_Kingdom, "United Kingdom" },
{ Country.United_States, "United States" },
{ Country.US_Minor_Outlying_Islands, "US Minor Outlying Islands" },
{ Country.Uruguay, "Uruguay" },
{ Country.Uzbekistan, "Uzbekistan" },
{ Country.Vanuatu, "Vanuatu" },
{ Country.Vatican_City_State_Holy_See, "Vatican City State (Holy See)" },
{ Country.Venezuela, "Venezuela" },
{ Country.Viet_Nam, "Viet Nam" },
{ Country.Virgin_Islands_British, "Virgin Islands (British)" },
{ Country.Virgin_Islands_US, "Virgin Islands (US)" },
{ Country.Wallis_and_Futuna_Islands, "Wallis and Futuna Islands" },
{ Country.Western_Sahara, "Western Sahara" },
{ Country.Yemen, "Yemen" },
{ Country.Yugoslavia, "Yugoslavia" },
{ Country.Zaire, "Zaire" },
{ Country.Zambia, "Zambia" },
{ Country.Zimbabwe, "Zimbabwe" }
};
			XEnum<Country>.SetNames(_names);
			XEnum<Country>.CaseInsensitive = true;
		}


		#endregion

		#region Code Generator

		/// <summary>
		/// Gets C# code generated from the current Countries and States 
		/// dictionaries for making them into enums.
		/// </summary>
		/// <returns></returns>
		public static string GetGeoNamesEnumsCode()
		{
			string countryEnums = XEnum.GetEnumCode("Country", Countries);
			string stateEnums = XEnum.GetEnumCode("State", States);

			string all = "\r\n" + countryEnums + "\r\n" + stateEnums + "\r\n";
			return all;
		}

		#endregion




	}

	#region Country and State Enums

	public enum Country
	{
		Afghanistan = 0,
		Albania = 1,
		Algeria = 2,
		American_Samoa = 3,
		Andorra = 4,
		Angola = 5,
		Anguilla = 6,
		Antarctica = 7,
		Antigua_and_Barbuda = 8,
		Argentina = 9,
		Armenia = 10,
		Aruba = 11,
		Australia = 12,
		Austria = 13,
		Azerbaijan = 14,
		Bahamas = 15,
		Bahrain = 16,
		Bangladesh = 17,
		Barbados = 18,
		Belarus = 19,
		Belgium = 20,
		Belize = 21,
		Benin = 22,
		Bermuda = 23,
		Bhutan = 24,
		Bolivia = 25,
		Bosnia_and_Herzegovina = 26,
		Botswana = 27,
		Bouvet_Island = 28,
		Brazil = 29,
		British_Indian_Ocean_Territory = 30,
		Brunei_Darussalam = 31,
		Bulgaria = 32,
		Burkina_Faso = 33,
		Burundi = 34,
		Cambodia = 35,
		Cameroon = 36,
		Canada = 37,
		Cape_Verde = 38,
		Cayman_Islands = 39,
		Central_African_Republic = 40,
		Chad = 41,
		Chile = 42,
		China = 43,
		Christmas_Island = 44,
		Cocos_Keeling_Islands = 45,
		Colombia = 46,
		Comoros = 47,
		Congo = 48,
		Cook_Islands = 49,
		Costa_Rica = 50,
		Cote_DIvoire_Ivory_Coast = 51,
		Croatia_Hrvatska = 52,
		Cuba = 53,
		Cyprus = 54,
		Czech_Republic = 55,
		Denmark = 56,
		Djibouti = 57,
		Dominica = 58,
		Dominican_Republic = 59,
		East_Timor = 60,
		Ecuador = 61,
		Egypt = 62,
		El_Salvador = 63,
		Equatorial_Guinea = 64,
		Eritrea = 65,
		Estonia = 66,
		Ethiopia = 67,
		Falkland_Islands_Malvinas = 68,
		Faroe_Islands = 69,
		Fiji = 70,
		Finland = 71,
		France = 72,
		France_Metropolitan = 73,
		French_Guiana = 74,
		French_Polynesia = 75,
		French_Southern_Territories = 76,
		Gabon = 77,
		Gambia = 78,
		Georgia = 79,
		Germany = 80,
		Ghana = 81,
		Gibraltar = 82,
		Greece = 83,
		Greenland = 84,
		Grenada = 85,
		Guadeloupe = 86,
		Guam = 87,
		Guatemala = 88,
		Guinea = 89,
		GuineaBissau = 90,
		Guyana = 91,
		Haiti = 92,
		Heard_and_McDonald_Islands = 93,
		Honduras = 94,
		Hong_Kong = 95,
		Hungary = 96,
		Iceland = 97,
		India = 98,
		Indonesia = 99,
		Iran = 100,
		Iraq = 101,
		Ireland = 102,
		Israel = 103,
		Italy = 104,
		Jamaica = 105,
		Japan = 106,
		Jordan = 107,
		Kazakhstan = 108,
		Kenya = 109,
		Kiribati = 110,
		Kuwait = 111,
		Kyrgyzstan = 112,
		Laos = 113,
		Latvia = 114,
		Lebanon = 115,
		Lesotho = 116,
		Liberia = 117,
		Libya = 118,
		Liechtenstein = 119,
		Lithuania = 120,
		Luxembourg = 121,
		Macau = 122,
		Macedonia = 123,
		Madagascar = 124,
		Malawi = 125,
		Malaysia = 126,
		Maldives = 127,
		Mali = 128,
		Malta = 129,
		Marshall_Islands = 130,
		Martinique = 131,
		Mauritania = 132,
		Mauritius = 133,
		Mayotte = 134,
		Mexico = 135,
		Micronesia = 136,
		Moldova = 137,
		Monaco = 138,
		Mongolia = 139,
		Montserrat = 140,
		Morocco = 141,
		Mozambique = 142,
		Myanmar = 143,
		Namibia = 144,
		Nauru = 145,
		Nepal = 146,
		Netherlands = 147,
		Netherlands_Antilles = 148,
		New_Caledonia = 149,
		New_Zealand = 150,
		Nicaragua = 151,
		Niger = 152,
		Nigeria = 153,
		Niue = 154,
		Norfolk_Island = 155,
		North_Korea = 156,
		Northern_Mariana_Islands = 157,
		Norway = 158,
		Oman = 159,
		Pakistan = 160,
		Palau = 161,
		Panama = 162,
		Papua_New_Guinea = 163,
		Paraguay = 164,
		Peru = 165,
		Philippines = 166,
		Pitcairn = 167,
		Poland = 168,
		Portugal = 169,
		Puerto_Rico = 170,
		Qatar = 171,
		Reunion = 172,
		Romania = 173,
		Russian_Federation = 174,
		Rwanda = 175,
		S_Georgia_and_S_Sandwich_Isls = 176,
		Saint_Kitts_and_Nevis = 177,
		Saint_Lucia = 178,
		Saint_Vincent_and_The_Grenadines = 179,
		Samoa = 180,
		San_Marino = 181,
		Sao_Tome_and_Principe = 182,
		Saudi_Arabia = 183,
		Senegal = 184,
		Seychelles = 185,
		Sierra_Leone = 186,
		Singapore = 187,
		Slovak_Republic = 188,
		Slovenia = 189,
		Solomon_Islands = 190,
		Somalia = 191,
		South_Africa = 192,
		Sourth_Korea = 193,
		Spain = 194,
		Sri_Lanka = 195,
		St_Helena = 196,
		St_Pierre_and_Miquelon = 197,
		Sudan = 198,
		Suriname = 199,
		Svalbard_and_Jan_Mayen_Islands = 200,
		Swaziland = 201,
		Sweden = 202,
		Switzerland = 203,
		Syria = 204,
		Taiwan = 205,
		Tajikistan = 206,
		Tanzania = 207,
		Thailand = 208,
		Togo = 209,
		Tokelau = 210,
		Tonga = 211,
		Trinidad_and_Tobago = 212,
		Tunisia = 213,
		Turkey = 214,
		Turkmenistan = 215,
		Turks_and_Caicos_Islands = 216,
		Tuvalu = 217,
		Uganda = 218,
		Ukraine = 219,
		United_Arab_Emirates = 220,
		United_Kingdom = 221,
		United_States = 222,
		US_Minor_Outlying_Islands = 223,
		Uruguay = 224,
		Uzbekistan = 225,
		Vanuatu = 226,
		Vatican_City_State_Holy_See = 227,
		Venezuela = 228,
		Viet_Nam = 229,
		Virgin_Islands_British = 230,
		Virgin_Islands_US = 231,
		Wallis_and_Futuna_Islands = 232,
		Western_Sahara = 233,
		Yemen = 234,
		Yugoslavia = 235,
		Zaire = 236,
		Zambia = 237,
		Zimbabwe = 238
	}

	public enum State
	{
		Alabama = 0,
		Alaska = 1,
		Arizona = 2,
		Arkansas = 3,
		California = 4,
		Colorado = 5,
		Connecticut = 6,
		Delaware = 7,
		Florida = 8,
		Georgia = 9,
		Hawaii = 10,
		Idaho = 11,
		Illinois = 12,
		Indiana = 13,
		Iowa = 14,
		Kansas = 15,
		Kentucky = 16,
		Louisiana = 17,
		Maine = 18,
		Maryland = 19,
		Massachusetts = 20,
		Michigan = 21,
		Minnesota = 22,
		Mississippi = 23,
		Missouri = 24,
		Montana = 25,
		Nebraska = 26,
		Nevada = 27,
		New_Hampshire = 28,
		New_Jersey = 29,
		New_Mexico = 30,
		New_York = 31,
		North_Carolina = 32,
		North_Dakota = 33,
		Ohio = 34,
		Oklahoma = 35,
		Oregon = 36,
		Pennsylvania = 37,
		Rhode_Island = 38,
		South_Carolina = 39,
		South_Dakota = 40,
		Tennessee = 41,
		Texas = 42,
		Utah = 43,
		Vermont = 44,
		Virginia = 45,
		Washington = 46,
		Washington_DC = 47,
		West_Virginia = 48,
		Wisconsin = 49,
		Wyoming = 50,
		Alberta = 51,
		British_Columbia = 52,
		Manitoba = 53,
		New_Brunswick = 54,
		Newfoundland = 55,
		Northwest_Territories = 56,
		Nova_Scotia = 57,
		Nunavut = 58,
		Ontario = 59,
		Prince_Edward_Island = 60,
		Quebec = 61,
		Saskatchewan = 62,
		Yukon = 63
	}

	#endregion

	//#region Country and State Enums

	//public enum Country
	//{
	//	Afghanistan,
	//	Albania,
	//	Algeria,
	//	American_Samoa,
	//	Andorra,
	//	Angola,
	//	Anguilla,
	//	Antarctica,
	//	Antigua_and_Barbuda,
	//	Argentina,
	//	Armenia,
	//	Aruba,
	//	Australia,
	//	Austria,
	//	Azerbaijan,
	//	Bahamas,
	//	Bahrain,
	//	Bangladesh,
	//	Barbados,
	//	Belarus,
	//	Belgium,
	//	Belize,
	//	Benin,
	//	Bermuda,
	//	Bhutan,
	//	Bolivia,
	//	Bosnia_and_Herzegovina,
	//	Botswana,
	//	Bouvet_Island,
	//	Brazil,
	//	British_Indian_Ocean_Territory,
	//	Brunei_Darussalam,
	//	Bulgaria,
	//	Burkina_Faso,
	//	Burundi,
	//	Cambodia,
	//	Cameroon,
	//	Canada,
	//	Cape_Verde,
	//	Cayman_Islands,
	//	Central_African_Republic,
	//	Chad,
	//	Chile,
	//	China,
	//	Christmas_Island,
	//	Cocos_Keeling_Islands,
	//	Colombia,
	//	Comoros,
	//	Congo,
	//	Cook_Islands,
	//	Costa_Rica,
	//	Cote_DIvoire_Ivory_Coast,
	//	Croatia_Hrvatska,
	//	Cuba,
	//	Cyprus,
	//	Czech_Republic,
	//	Denmark,
	//	Djibouti,
	//	Dominica,
	//	Dominican_Republic,
	//	East_Timor,
	//	Ecuador,
	//	Egypt,
	//	El_Salvador,
	//	Equatorial_Guinea,
	//	Eritrea,
	//	Estonia,
	//	Ethiopia,
	//	Falkland_Islands_Malvinas,
	//	Faroe_Islands,
	//	Fiji,
	//	Finland,
	//	France,
	//	France_Metropolitan,
	//	French_Guiana,
	//	French_Polynesia,
	//	French_Southern_Territories,
	//	Gabon,
	//	Gambia,
	//	Georgia,
	//	Germany,
	//	Ghana,
	//	Gibraltar,
	//	Greece,
	//	Greenland,
	//	Grenada,
	//	Guadeloupe,
	//	Guam,
	//	Guatemala,
	//	Guinea,
	//	GuineaBissau,
	//	Guyana,
	//	Haiti,
	//	Heard_and_McDonald_Islands,
	//	Honduras,
	//	Hong_Kong,
	//	Hungary,
	//	Iceland,
	//	India,
	//	Indonesia,
	//	Iran,
	//	Iraq,
	//	Ireland,
	//	Israel,
	//	Italy,
	//	Jamaica,
	//	Japan,
	//	Jordan,
	//	Kazakhstan,
	//	Kenya,
	//	Kiribati,
	//	Kuwait,
	//	Kyrgyzstan,
	//	Laos,
	//	Latvia,
	//	Lebanon,
	//	Lesotho,
	//	Liberia,
	//	Libya,
	//	Liechtenstein,
	//	Lithuania,
	//	Luxembourg,
	//	Macau,
	//	Macedonia,
	//	Madagascar,
	//	Malawi,
	//	Malaysia,
	//	Maldives,
	//	Mali,
	//	Malta,
	//	Marshall_Islands,
	//	Martinique,
	//	Mauritania,
	//	Mauritius,
	//	Mayotte,
	//	Mexico,
	//	Micronesia,
	//	Moldova,
	//	Monaco,
	//	Mongolia,
	//	Montserrat,
	//	Morocco,
	//	Mozambique,
	//	Myanmar,
	//	Namibia,
	//	Nauru,
	//	Nepal,
	//	Netherlands,
	//	Netherlands_Antilles,
	//	New_Caledonia,
	//	New_Zealand,
	//	Nicaragua,
	//	Niger,
	//	Nigeria,
	//	Niue,
	//	Norfolk_Island,
	//	North_Korea,
	//	Northern_Mariana_Islands,
	//	Norway,
	//	Oman,
	//	Pakistan,
	//	Palau,
	//	Panama,
	//	Papua_New_Guinea,
	//	Paraguay,
	//	Peru,
	//	Philippines,
	//	Pitcairn,
	//	Poland,
	//	Portugal,
	//	Puerto_Rico,
	//	Qatar,
	//	Reunion,
	//	Romania,
	//	Russian_Federation,
	//	Rwanda,
	//	S_Georgia_and_S_Sandwich_Isls,
	//	Saint_Kitts_and_Nevis,
	//	Saint_Lucia,
	//	Saint_Vincent_and_The_Grenadines,
	//	Samoa,
	//	San_Marino,
	//	Sao_Tome_and_Principe,
	//	Saudi_Arabia,
	//	Senegal,
	//	Seychelles,
	//	Sierra_Leone,
	//	Singapore,
	//	Slovak_Republic,
	//	Slovenia,
	//	Solomon_Islands,
	//	Somalia,
	//	South_Africa,
	//	Sourth_Korea,
	//	Spain,
	//	Sri_Lanka,
	//	St_Helena,
	//	St_Pierre_and_Miquelon,
	//	Sudan,
	//	Suriname,
	//	Svalbard_and_Jan_Mayen_Islands,
	//	Swaziland,
	//	Sweden,
	//	Switzerland,
	//	Syria,
	//	Taiwan,
	//	Tajikistan,
	//	Tanzania,
	//	Thailand,
	//	Togo,
	//	Tokelau,
	//	Tonga,
	//	Trinidad_and_Tobago,
	//	Tunisia,
	//	Turkey,
	//	Turkmenistan,
	//	Turks_and_Caicos_Islands,
	//	Tuvalu,
	//	Uganda,
	//	Ukraine,
	//	United_Arab_Emirates,
	//	United_Kingdom,
	//	United_States,
	//	US_Minor_Outlying_Islands,
	//	Uruguay,
	//	Uzbekistan,
	//	Vanuatu,
	//	Vatican_City_State_Holy_See,
	//	Venezuela,
	//	Viet_Nam,
	//	Virgin_Islands_British,
	//	Virgin_Islands_US,
	//	Wallis_and_Futuna_Islands,
	//	Western_Sahara,
	//	Yemen,
	//	Yugoslavia,
	//	Zaire,
	//	Zambia,
	//	Zimbabwe
	//}

	//public enum State
	//{
	//	Alabama = 0,
	//	Alaska,
	//	Arizona,
	//	Arkansas,
	//	California,
	//	Colorado,
	//	Connecticut,
	//	Delaware,
	//	Florida,
	//	Georgia,
	//	Hawaii,
	//	Idaho,
	//	Illinois,
	//	Indiana,
	//	Iowa,
	//	Kansas,
	//	Kentucky,
	//	Louisiana,
	//	Maine,
	//	Maryland,
	//	Massachusetts,
	//	Michigan,
	//	Minnesota,
	//	Mississippi,
	//	Missouri,
	//	Montana,
	//	Nebraska,
	//	Nevada,
	//	New_Hampshire,
	//	New_Jersey,
	//	New_Mexico,
	//	New_York,
	//	North_Carolina,
	//	North_Dakota,
	//	Ohio,
	//	Oklahoma,
	//	Oregon,
	//	Pennsylvania,
	//	Rhode_Island,
	//	South_Carolina,
	//	South_Dakota,
	//	Tennessee,
	//	Texas,
	//	Utah,
	//	Vermont,
	//	Virginia,
	//	Washington,
	//	Washington_DC,
	//	West_Virginia,
	//	Wisconsin,
	//	Wyoming,
	//	Alberta,
	//	British_Columbia,
	//	Manitoba,
	//	New_Brunswick,
	//	Newfoundland,
	//	Northwest_Territories,
	//	Nova_Scotia,
	//	Nunavut,
	//	Ontario,
	//	Prince_Edward_Island,
	//	Quebec,
	//	Saskatchewan,
	//	Yukon
	//}

	//#endregion

}
