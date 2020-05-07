using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    /// <summary>
    /// Describes the outcome of an attemped analysis by SituationOperator.
    /// Third-digit values are used in analogy to HTTP statuscodes, meaning 2XX indicates success, 
    /// 4XX indicates client failure (e.g. bad instructions), and 5XX indicates server failure (e.g. unhandled exceptions)
    /// </summary>
    public enum SituationOperatorResult : int
    {
        Success = 200,

        RedisKeyExpiredError = 401,

        UnknownError = 500,
        RedisUnavailableError = 501,
        UploadToDatabaseError = 502,
    }
}
