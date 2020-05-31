using System.Text;

namespace IsUakr.MessageHandler.DAL
{
    public static class Utilities
    {
        public static string CreateMeterMessageTable(string tableName)
        {
            return $"create table public.\"{tableName}\"(id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1) , meterdt timestamp with time zone NOT NULL, meterid integer NOT NULL, serial integer NOT NULL, volume numeric NOT NULL, unit integer NOT NULL, PRIMARY KEY(id))" +
                $"WITH (OIDS = FALSE); ALTER TABLE public.{tableName} OWNER to ukbacjfmdnmimz; GRANT ALL PRIVILEGES ON TABLE public.{tableName} TO ukbacjfmdnmimz;";
        }
    }
}
