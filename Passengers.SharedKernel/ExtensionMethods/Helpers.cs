using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Pagnation;
using Passengers.SharedKernel.SharedDto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.ExtensionMethods
{
    public static class Helpers
    {
        //Exception
        public static string ToFullException(this System.Exception exception)
        {
            StringBuilder FullMessage = new();
            return Recursive(exception);
            //local function
            string Recursive(System.Exception deep)
            {
                FullMessage.Append(Environment.NewLine + deep.ToString() + Environment.NewLine + deep.Message);
                if (deep.InnerException is null) return FullMessage.ToString();
                return Recursive(deep.InnerException);
            }

        }


        //string
        public static bool IsNullOrEmpty(this string value)
            => System.String.IsNullOrEmpty(value);

        public static T ToEnum<T>(this string value)
            => (T)Enum.Parse(typeof(T), value, true);

        //public static List<SelectDto> EnumToList<T>() where T : Enum
        //{
        //    return Enum.GetValues(typeof(T))
        //       .Cast<T>()
        //       .Select(t => new SelectDto
        //       {
        //           Id = (int)t,
        //           Name = t.ToString()
        //       }).ToList();
        //}

        //boolean
        public static string NestedIF(this bool value, string trueSide, string falseSide)
         => value ? trueSide : falseSide;
        public static T NestedIF<T>(this bool value, T trueSide, T falseSide)
        => value ? trueSide : falseSide;


        //object
        public static T CastTo<T>(this object value)
          => (T)value;

        public static T AsTo<T>(this object value) where T : class
          => value as T;

        public static TCast GetValueReflection<TObject, TCast>(this TObject obj, string propertyName)
          => ((Func<TObject, TCast>)Delegate.CreateDelegate(typeof(Func<TObject, TCast>), null, typeof(TObject).GetProperty(propertyName).GetGetMethod()))(obj).CastTo<TCast>();

        public static TGetterResult GetValueReflection<TObject, TGetterResult>(string propertyName)
        => ((Func<TGetterResult>)Delegate.CreateDelegate(typeof(Func<TGetterResult>), null, typeof(TObject).GetProperty(propertyName).GetGetMethod()))().CastTo<TGetterResult>();

        //claims
        public static object CurrentUserId(this System.Security.Claims.ClaimsPrincipal user) =>
             user.FindFirst(ClaimTypes.NameIdentifier).Value;

        public static T CurrentUserId<T>(this System.Security.Claims.ClaimsPrincipal user) =>
            (T)Convert.ChangeType(user.FindFirst(ClaimTypes.NameIdentifier).Value, typeof(T));


        public static T CurrentUserId<T>(this IEnumerable<Claim> claims)
            => (T)Convert.ChangeType(claims.First(claim => claim.Type == (ClaimTypes.NameIdentifier)).Value, typeof(T));

        public static T CurrentUserRole<T>(this IEnumerable<Claim> claims) =>
            claims.First(claim => claim.Type == (ClaimTypes.Role)).Value.ToEnum<T>();

        public static string CurrentUserName(this IEnumerable<Claim> claims) =>
             claims.First(claim => claim.Type == nameof(ClaimTypes.Name)).Value;

        public static Guid? StringToGuid(this string source) =>
             source is null ? new Guid ("00000000-0000-0000-0000-000000000000") : Guid.Parse(source);

        // Random
        public static string GetFourNumberToken()
        {
            Random random = new();
            var token = "";
            int c = 0;
            while (c < 4)
            {
                int x = random.Next(0, 9);
                token += x.ToString();
                c++;
            }
            return token;
        }

        internal static readonly char[] chars =
          "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        public static string GetUniqueKey(int size = 6)
        {
            byte[] data = new byte[4 * size];
            using (System.Security.Cryptography.RNGCryptoServiceProvider crypto = new())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }


        //Enumrable
        public static (IEnumerable<T> Add, IEnumerable<T> remove) IsolatedExcept<T>(this IEnumerable<T> @old, IEnumerable<T> @new)
            => (@new.Except(old), old.Except(@new));

        public static (IEnumerable<TProp> Add, IEnumerable<TProp> remove) IsolatedExcept<Told, Tnew, TProp>(this IEnumerable<Told> @old, IEnumerable<Tnew> @new, Func<Told, TProp> funcold, Func<Tnew, TProp> funcnew)
            => @old.Select(funcold).IsolatedExcept(@new.Select(funcnew));

        public static (IEnumerable<Tnew> Add, IEnumerable<Told> remove) IsolatedExceptOldNew<Told, Tnew, TProp>(this IEnumerable<Told> @old, IEnumerable<Tnew> @new, Func<Told, TProp> funcold, Func<Tnew, TProp> funcnew)
        {
            ICollection<Tnew> Add = new Collection<Tnew>();
            ICollection<Told> remove = new List<Told>(@old);
            foreach (var item in @new)
            {
                var _old = old.Select(funcold).SingleOrDefault(x => x.Equals(funcnew(item)));
                if (_old is null || !old.Any() || _old.ToString() == "00000000-0000-0000-0000-000000000000")
                    Add.Add(item);
                else
                    remove.Remove(old.First(x => funcold(x).Equals(_old)));
            }
            return (Add, remove);
        }

        public static bool AnyIsNullOrEmpty(this IEnumerable<string> source) => source is null || source.Any(s => s.IsNullOrEmpty());

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source is null || !source.Any();

        public static IQueryable<T> Pagnation<T>(this IQueryable<T> source, int pageSize, int pageNumber)
            => source.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

        public static double CustomAverage<T>(this List<T> source, Func<T, int> func)
            => source.Any() ? source.Average(func) : 0;

        public static double CustomAverage<T>(this List<T> source, Func<T, double> func)
            => source.Any() ? source.Average(func) : 0;

        public static double CustomAverage<T>(this ICollection<T> source, Func<T, int> func)
            => source.Any() ? source.Average(func) : 0;

        public static double CustomAverage<T>(this ICollection<T> source, Func<T, double> func)
            => source.Any() ? source.Average(func) : 0;

        //Enum
        public static List<SelectDto> EnumToList(Type enumType)
        {
            if (!typeof(Enum).IsAssignableFrom(enumType))
                throw new ArgumentException("enumType should describe enum");

            var names = Enum.GetNames(enumType).Cast<object>();
            var values = Enum.GetValues(enumType).Cast<int>();

            var list = names.Zip(values).ToList();

            return list.Select(x => new SelectDto
            {
                Id = x.Second,
                Name = x.First.ToString()
            }).ToList();

        }

        


    }
}
