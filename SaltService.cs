using HashidsNet;
using System.Text;

namespace simplesalt
{
    public class SaltService
    {
        private readonly ILogger logger;
        private readonly Dictionary<string,string> saltMaps;


        public SaltService(ILogger<SaltService> logger, IConfiguration configuration)
        {
            this.logger = logger;

            var mappingConfig = configuration["SaltMapping"] is not null ? configuration["SaltMapping"] : Environment.GetEnvironmentVariable("SALTMAPPING");

            if(mappingConfig is null)
            {
                throw new ApplicationException("Could not get salt mapping configuration");
            }

            saltMaps = ExtractSaltMaps(mappingConfig);
        }

        private Dictionary<string, string> ExtractSaltMaps(string mapconfig)
        {
            Dictionary<string, string> saltMap = new Dictionary<string, string>();
            List<string> pairs = mapconfig.Split(';').ToList();
            foreach (var pair in pairs)
            {
                List<string> split = pair.Split(':').ToList();
                
                if(string.IsNullOrEmpty(split[0]))
                {
                    throw new ApplicationException("Trying to extract salt mapping key returned an empty string.");
                }

                if (string.IsNullOrEmpty(split[1]))
                {
                    throw new ApplicationException("Trying to extract salt mapping value returned an empty string.");
                }

                var envValue = Environment.GetEnvironmentVariable(split[1]);

                if (string.IsNullOrEmpty(envValue))
                {
                    throw new ApplicationException($"Trying to extract environment variable for mapping field: {split[1]} returned an empty string.");
                }

                saltMap.Add(split[0], envValue);
            }
            return saltMap;
        }

        public string Encode(string sourceValue, string contentType)
        {
            logger.LogInformation($"Encoding {sourceValue} for type={contentType}");
            var hashids = new Hashids(saltMaps[contentType.ToUpper()]);
            string hexString = Convert.ToHexString(Encoding.Unicode.GetBytes(sourceValue));
            return hashids.EncodeHex(hexString);
        }

        public string Decode(string sourceValue, string contentType)
        {
            logger.LogInformation($"Decoding {sourceValue} for type={contentType}");
            var hashids = new Hashids(saltMaps[contentType.ToUpper()]);
            var decoded = hashids.DecodeHex(sourceValue);
            byte[] bb = Enumerable.Range(0, decoded.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(decoded.Substring(x, 2), 16))
                     .ToArray();
            return Encoding.Unicode.GetString(bb);
        }

        public bool ContentTypeExist(string contentType)
        {
            return saltMaps.ContainsKey(contentType.ToUpper());
        }
    }


}
