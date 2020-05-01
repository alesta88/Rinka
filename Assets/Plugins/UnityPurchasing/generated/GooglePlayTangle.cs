#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SPaP/9+FUwxXIU0I9mc8OST+Q7gU88fRiej5gsrL4QsLdzPnSnWPrr3CwTJ/NvBGuOSoUHiu15/8oJBn4JCjZl74utsti2ExLZlFP2bVG8UpqqSrmymqoakpqqqrPOeb9jIHVxDMQ/1iU1QRIVdVMxVPyOuOflt7vjZLSHrzelvwbFawOAqFM4VLr2dWKp43aySHXVzQhz7vQee/YhFYthRLHG132GwqfUvA4sGIAjWze8qI9vR04ypcvlZNQ7eN9sq8LnfTRkE6W6gp2qArZQKngk5FADh1grZyLpspqombpq2igS3jLVymqqqqrquoli7l2/pYFcUnIWwcxfYc+zmOnnUXbwLhmWaoOhEO/dM1oz982uiN+lQxt2mEdvZdjqmoqquq");
        private static int[] order = new int[] { 6,5,7,11,5,11,8,10,10,13,13,13,13,13,14 };
        private static int key = 171;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
