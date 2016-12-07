namespace Test.Unit.TS
{
    using System.IO;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;
    using RiakClient.Util;

    public class StoreTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_PB_Req()
        {
            Store cmd = BuildStoreReq();
            Assert.AreEqual(MessageCode.TsPutResp, cmd.ExpectedCode);

            TsPutReq pb = (TsPutReq)cmd.ConstructRequest(useTtb: false);
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));

            CollectionAssert.AreEqual(TsCols, pb.columns);
            CollectionAssert.AreEqual(TsRows, pb.rows);
        }

        [Test]
        public void Should_Build_TTB_Req()
        {
            Store cmd = BuildStoreReq();
            Assert.AreEqual(MessageCode.TsPutResp, cmd.ExpectedCode);

            TsTtbMsg ttb = (TsTtbMsg)cmd.ConstructRequest(useTtb: true);
            Assert.AreEqual(MessageCode.TsTtbMsg, cmd.ExpectedCode);

            byte[] want =
            {
                131, 104, 4, // 4-tuple
                    100, 0, 8, 116, 115, 112, 117, 116, 114, 101, 113,
                    109, 0, 0, 0, 10, 71, 101, 111, 67, 104, 101, 99, 107, 105, 110,
                    106, // empty columns
                    108, 0, 0, 0, 2, // 2-list
                        104, 6, // 6-tuple (row)
                            100, 0, 5, 102, 97, 108, 115, 101, // false atom
                            /*
                            4> f(Bin), Bin = <<131,70,64,40,174,20,128,0,0,0>>, binary_to_term(Bin).
                            12.34000015258789
                            */
                            70, 64, 40, 174, 20, 128, 0, 0, 0, // 12.34 float as double, note that this is how .NET calcs it, NOT erl
                            97, 32, // small int
                            110, 6, 0, 185, 134, 44, 95, 81, 1, // smallbig 1449000732345
                            109, 0, 0, 0, 6, 102, 111, 111, 98, 97, 114, // binary
                            109, 0, 0, 0, 6, 102, 111, 111, 98, 97, 114, // binary
                        104, 6, // 6-tuple (row)
                            100, 0, 4, 116, 114, 117, 101,  // true atom
                            70, 64, 76, 99, 215, 0, 0, 0, 0, // 56.78 float as double
                            97, 54, // small int
                            110, 6, 0, 65, 154, 44, 95, 81, 1, // smallbig 1449000737345
                            109, 0, 0, 0, 6, 98, 97, 122, 98, 97, 116, // binary
                            109, 0, 0, 0, 6, 98, 97, 122, 98, 97, 116, // binary
                    106 // 2-list end
            };

            byte[] got;
            using (var ms = new MemoryStream())
            {
                ttb.WriteTo(ms);
                ms.Flush();
                got = ms.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }
    }
}
