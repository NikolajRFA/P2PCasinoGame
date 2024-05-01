using Peer;

namespace TestProject1;

public class CommunicationHandlerTests
{
    [Fact]
    public void BuildParameters_ListOfIndexesParamsInts_Correct()
    {
        Assert.Equal("[1,2];1;2", CommunicationHandler.BuildParameters([1,2], 1,2));
    }

    [Fact]
    public void BuildParameters_SingleInt_Correct()
    {
        Assert.Equal("1", CommunicationHandler.BuildParameters(1));
    }
}