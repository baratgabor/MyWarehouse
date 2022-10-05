namespace MyWarehouse.Infrastructure.Authorization.Constants;

public static class KnownClaims
{
    public static class ExampleClaim
    {
        public static string Name => nameof(ExampleClaim);

        public static class Values
        {
            public static string ExampleValue1 => nameof(ExampleValue1);
            public static string ExampleValue2 => nameof(ExampleValue2);
        }
    }
}
