﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.Imaging
{
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class FastRetinaKeypointDetectorTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void ProcessImageTest()
        {
            Bitmap lena = Properties.Resources.lena512;

            FastRetinaKeypointDetector target = new FastRetinaKeypointDetector();

            List<FastRetinaKeypoint> actual = target.ProcessImage(lena);

            string code;
            Assert.AreEqual(1283, actual.Count);

            int i = 0;
            Assert.AreEqual(223, actual[i].X);
            Assert.AreEqual(45, actual[i].Y);
            Assert.AreEqual(80.122163710144434, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("00101100101000101100000010111000110111111010111011110000011011110111111111101001011000111111010110100110101001000010100000001100100000111110110100000010100010010000100000110000100010110000000101110001100100101100011010111101100100110101111110000100010100111101100001010101110010000001100111111010100001111000111001101000011101111011110111001101111010101101001111110101010100000100100100101100101010011010011000000000001010000110101010011000111001111011111101110101110011100001001010110010100100001000101001010011", code);
            code = actual[i].ToHex();
            Assert.AreEqual("3445031dfb750ff6fe97c6af65251430c1b74091100cd1808e4963bdc9fa21ca1baa13985fe17116eebdb357cbaf0a9234956500145619e7fdae73484d0951ca", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("NEUDHft1D/b+l8avZSUUMMG3QJEQDNGAjkljvcn6IcobqhOYX+FxFu69s1fLrwqSNJVlABRWGef9rnNITQlRyg==", code);

            i = 124;
            Assert.AreEqual(141.0, actual[i].X);
            Assert.AreEqual(184.0, actual[i].Y);
            Assert.AreEqual(158.74949449286677, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("11111000010001010000011000000010000100100010010101000010100000011110101010110111001110010110111110010101100001000100010100001000110010000001010000010000000001010011010110000010101110110000001000010000100000010101011010111011110000001011101110001011100001110010000001000000001011000010100010001001100001101000010100001000000001001010110100000000001010011101001010000111011010011000110010000100110000100010011001010000001010010110001000010000111000011000000000100100110000100001001000010010100100000000100001000001", code);
            code = actual[i].ToHex();
            Assert.AreEqual("1fa2604048a4428157ed9cf6a921a210132808a0ac41dd4008816add03ddd1e1040234149161a11020b500944be196312143640a944608870124434848091082", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("H6JgQEikQoFX7Zz2qSGiEBMoCKCsQd1ACIFq3QPd0eEEAjQUkWGhECC1AJRL4ZYxIUNkCpRGCIcBJENISAkQgg==", code);

            i = 763;
            Assert.AreEqual(104, actual[i].X);
            Assert.AreEqual(332, actual[i].Y);
            Assert.AreEqual(-174.28940686250036, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("01010001111011001100011000010110101011111010010001000101001111110111111101101101111000101101010111010110111010001110001011011100111000010000010111100000010010010101010001010001110100010101111011000001101100111001001100001000101100010111011110111100111101110100011000100101001010010011010000111110000100000000000011001000000011111011010011011001001010011101010011111101001001010111000001010100011011110111010000010101100000010110110101111100101111111000100011111001100011100000110011011011100000001010100101011101", code);
            code = actual[i].ToHex();
            Assert.AreEqual("8a376368f525a2fcfeb647ab6b17473b87a007922a8a8b7a83cdc9108dee3def62a4942c7c080013f02d9b942bbfa40e2af62ea881b63efd119f7130db0195ba", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("ijdjaPUlovz+tkeraxdHO4egB5Iqiot6g83JEI3uPe9ipJQsfAgAE/Atm5Qrv6QOKvYuqIG2Pv0Rn3Ew2wGVug==", code);

            i = 1042;
            Assert.AreEqual(116, actual[i].X);
            Assert.AreEqual(410, actual[i].Y);
            Assert.AreEqual(-86.11209043916692, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("11110111010011000010101001011011010101100111010011011101001111010110001010110000111001010111101011001011001001101101100011011011110111110000010111111000000000010001001000010110101010010001100011011110101000100101100010101100000100111011101100010110001000111000100001110100001000110001110011111011000001100000111001001000011111111011010101001101110000010101001110111101011100001100110110000100111010100010000001000000011110000110111010011000111100111011111110110101110011100000001001110010100100001000101001110011", code);
            code = actual[i].ToHex();
            Assert.AreEqual("ef3254da6a2ebbbc460da75ed3641bdbfba01f80486895187b451a35c8dd68c4112ec438df607012feadb283cabd0eb3215704021e7619cffdad73404e0951ce", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("7zJU2mouu7xGDade02Qb2/ugH4BIaJUYe0UaNcjdaMQRLsQ432BwEv6tsoPKvQ6zIVcEAh52Gc/9rXNATglRzg==", code);

            i = 1282;
            Assert.AreEqual(425.0, actual[i].X);
            Assert.AreEqual(488.0, actual[i].Y);
            Assert.AreEqual(-70.722526916337571, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("10111011111111011101111000011111111101110111111011111111111111111111111111111011011110111111111111111111011011101001111101101110111101111111101111111110111011101111111010110011100100111110101101111111111110111111111110111111111111111111011011101101110111111111111010110101110111000001010101111111100111011000111111110111011100111111111111111101111110101111111111110111010101000111011100111111101011011011111100011011111011111111101111111001111011111011111111110111111111110001001110111110100110101111111001010111", code);
            code = actual[i].ToHex();
            Assert.AreEqual("ddbf7bf8ef7effffffdfdeffff76f976efdf7f777fcdc9d7fedffffdff6fb7fb7fad3ba8feb9f1efceffbf5fffef2aeefcb5fdd8f7df9ff7fdefffc87d597fea", code);
        }

        [TestMethod()]
        public void ProcessImageTest2()
        {
            Bitmap lena = Properties.Resources.lena512;

            FastRetinaKeypointDetector target = new FastRetinaKeypointDetector();
            target.ComputeDescriptors = FastRetinaKeypointDescriptorType.Extended;

            List<FastRetinaKeypoint> actual = target.ProcessImage(lena);

            string code;
            Assert.AreEqual(1283, actual.Count);

            int i = 0;
            Assert.AreEqual(223, actual[i].X);
            Assert.AreEqual(45, actual[i].Y);
            Assert.AreEqual(80.122163710144434, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("0000000000011110111100011100000000000000100010011100111111111111111111111111100111100111100000010001100000000100010000000000000000000000011111111110011110111111111100111100011100111100011100000000000000000000000000000000000000000000011100111000011100011111111111110111111111111111111111100111111111100111100111100111100111100001110001100001110001100000000000000000000000000000000000100010000000100011000001011110011110001110011110001111111111111100111111111100111110011100111100011100111100011100000010001000000010001100000100000000100010000000100011000001000000011100111100011100111100011100011111111111110011111111110011111111111111111111100111111111100111110111100111111111100111100111100111110111100001110001100001110001100001110001100000001100011000011100011000001100011000000011100011000011100011000011100011000011011111111110011110011110011111011110001110111100111100011100111100011100111100011100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", code);
            code = actual[i].ToHex();
            Assert.AreEqual("00788f030091f3ffff9fe7811820020000fee7fdcfe33c0e0000000000cee1f8fffeff7ffee7799e876338060000004004c4a0e7711eff3ffff3398ff3381001310810013108388ff338fe3ffff3fffff99feff99fe7f91e8ee1188e0163380663c0311cc331ec7f9ee77bdcf3388ff338000000000000000000000000000000", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("AHiPAwCR8///n+eBGCACAAD+5/3P4zwOAAAAAADO4fj//v9//ud5nodjOAYAAABABMSg53Ee/z//8zmP8zgQATEIEAExCDiP8zj+P//z///5n+/5n+f5Ho7hGI4BYzgGY8AxHMMx7H+e53vc8ziP8zgAAAAAAAAAAAAAAAAAAAA=", code);

            i = 124;
            Assert.AreEqual(141.0, actual[i].X);
            Assert.AreEqual(184.0, actual[i].Y);
            Assert.AreEqual(158.74949449286677, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("0000000001001110111110011100000110000000000000011100011001111011110011100111100011100111100001110001110000001000001000000011100011000001001110001110000110011100011100011110011100011100001100001110001110000110000001100011000001000000011100011100001100111001110001100000100000100001100001000001000000000001100001000001000000001001110001100000100000101110001100011000001000001001100011100011100001100111011111001110001100000100000100110100001000001000000000000000000000001000001000000000000000000001001110001100000100000100110100110011100011000001100001011111011110011100011100001110111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000110001100000100000000110000110001110001100011000001000001001100001100011110011100011100001110111111111111111011111001110001100000100000100110100110001111100001000001000000000000000000000000011100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", code);
            code = actual[i].ToHex();
            Assert.AreEqual("00729f83018063de731ee7e13810041c831c87398ee7380cc761600c028ec39c6310842108802108906310748c4190711ce63ec720c842100000100400801c8320cb1c83a1ef390ef7ff07000000000000000000000000408c4180611c63106418cf71b8ffbfcf3108b28c0f4100000007000000000000000000000000000000", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("AHKfgwGAY95zHufhOBAEHIMchzmO5zgMx2FgDAKOw5xjEIQhCIAhCJBjEHSMQZBxHOY+xyDIQhAAABAEAIAcgyDLHIOh7zkO9/8HAAAAAAAAAAAAAAAAQIxBgGEcYxBkGM9xuP+/zzEIsowPQQAAAAcAAAAAAAAAAAAAAAAAAAA=", code);

            i = 763;
            Assert.AreEqual(104, actual[i].X);
            Assert.AreEqual(332, actual[i].Y);
            Assert.AreEqual(-174.28940686250036, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("0000010000000010010110010110000011000000000000000000001000011000110000110001100000110001111011111111111110000000000000000000000000000000011111111111111110000000000000001100000000000000000100000011000110100110110000000000000000000000000000001000001101101011111111111110110111110000000000000000100001000000000000000000000000000111111111111111111111111110000000000000000000001000100000000000000000000001000101111111111111111111111111110110000000000000000000001000100000111111111111111111111111101101111111111111111111111111111111110000000000000001100101001101101001111111111111111111111111101101101111111111111111111111111111111111110011111111111101101111101101101001000000000000000000100001001101101000000111111111111111111111111111111111111110010110111111101101111101101101001000100000110001100001101101101101101001000100111111111111111111111111111111111111111111111111111111111111111111111111111111111100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", code);
            code = actual[i].ToHex();
            Assert.AreEqual("20409a0603004018c3188cf7ff01000000feff01000300088c6503000000c1d6ffb70f0010020000e0ffff7f000010010080e8ffffff06000011fcffffb7ffffffff0080295bfeffffb7fdffffff3fff6fdf960000846c81ffffffff9ff6b76f4b0463d8b625f2ffffffffffffffffff3f000000000000000000000000000000", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("IECaBgMAQBjDGIz3/wEAAAD+/wEAAwAIjGUDAAAAwdb/tw8AEAIAAOD//38AABABAIDo////BgAAEfz//7f/////AIApW/7//7f9////P/9v35YAAIRsgf////+f9rdvSwRj2LYl8v///////////z8AAAAAAAAAAAAAAAAAAAA=", code);

            i = 1042;
            Assert.AreEqual(116, actual[i].X);
            Assert.AreEqual(410, actual[i].Y);
            Assert.AreEqual(-86.11209043916692, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("0000000001001111111110011100000000000001100010011100111000110001000001100010000011110111111000110001000000000000000000000001100010000011001110011101101110011110111111011110000000000000001000000100001000000100010000000000000000000000001100010001011100111000110001000001100011100011100111011011100111110011110111111011110111111000000000000000100000100000000000000000000000000000000001100010011011100111110011001111011111101111011111111111111111111111111111111111111110001100010011011100111110011000000000000000000100010100001100000000000000000001000001000001000000011110111111111111111111111110111001111011111101111111111111111011101111111111111111111111111111110111111111111111111111111111111111110111111000110001000001100011101001100001100000000000000000001000001000001000000000000011100111011011100111111011100111000011111111011111111111111111111111011111001111111110111111111111111111111110111110011100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", code);
            code = actual[i].ToHex();
            Assert.AreEqual("00f29f038091738c6004efc708000018c19cdb79bf07000442200200008ce81c2318c7b99dcffbbd1f0010040000006064e733eff7feffffffff31b2f3190080280c0080200878ffff7fe7fdfeffddffffffefffffffff7e8c605c860100100401c0b99ddf39fcfbfffffbfcf7fffff739000000000000000000000000000000", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("APKfA4CRc4xgBO/HCAAAGMGc23m/BwAEQiACAACM6BwjGMe5nc/7vR8AEAQAAABgZOcz7/f+/////zGy8xkAgCgMAIAgCHj//3/n/f7/3f///+//////foxgXIYBABAEAcC5nd85/Pv///v89///9zkAAAAAAAAAAAAAAAAAAAA=", code);

            i = 1282;
            Assert.AreEqual(425.0, actual[i].X);
            Assert.AreEqual(488.0, actual[i].Y);
            Assert.AreEqual(-70.722526916337571, actual[i].Orientation);
            code = actual[i].ToBinary();
            Assert.AreEqual("0000110000000000111110000010011011010110110110000000000011111011111111111111110111110111100000011010010000111110111100010110110110100010011111011110011111111111111111111110111110111100011100011111011110001110000111110111100011100010111110111100111110111111111111111111111011111111111111111111111111111111111111110111110111100011111011110011111011110000111110111100111110111100010111110111100111110111100011111111111111011111011110001111111111111111111111111110111110111111111110111110111100011100011111011111011111011110001110000111111111110111110111100011100010111111111110111110111100011100111111111111111111111111111011111011111111111111111111111111111111111111111111111111111111111111110111110111100111111111111011111011110001110011110000111111111110111110111100011100011000001111111111110111110111100011100111100011111111111111111111111111011111011110011111111111111111111111111110111110111100111100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", code);
            code = actual[i].ToHex();
            Assert.AreEqual("30001f646b1b00dfffbfef81257c8fb645bee7fffff73d8eef71f81e47dff3fdff7fffffffffffbec7f77c0fdff33dfa9eeff1fffb1efffffff7fddff738beef7b1cfeef7b1cfddff738fffffff7fdfffffffffffffffb9efff73dcec3ff7d8f63f0ffbec779fcffffef7bfeffffdff73c000000000000000000000000000000", code);
            code = actual[i].ToBase64();
            Assert.AreEqual("MAAfZGsbAN//v++BJXyPtkW+5///9z2O73H4Hkff8/3/f///////vsf3fA/f8z36nu/x//se////9/3f9zi+73sc/u97HP3f9zj////3/f/////////7nv/3Pc7D/32PY/D/vsd5/P//73v+///f9zwAAAAAAAAAAAAAAAAAAAA=", code);
        }
    }
}
