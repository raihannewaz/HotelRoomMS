namespace Common.Core.IdsGenerator;

public static class CurrentDateTimeCountIdGenerator
{
    public static long Id()
    {
       // return Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssffff"));
       return SnowFlakIdGenerator.NewId();
    }
}
