using blog.common.Model;
using System.Diagnostics.CodeAnalysis;

namespace blog.common.Middleware
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method)]
    public class HasPermission : Attribute
    {
        public readonly UserRole Role;

        public HasPermission(UserRole role) {  Role = role; }
    }
}
