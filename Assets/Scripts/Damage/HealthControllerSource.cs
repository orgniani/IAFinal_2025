using UnityEngine;
using DataSource;

namespace Damage
{
    [CreateAssetMenu(menuName = "DataSources/Health Controller", fileName = "Source_HealthControllerData")]
    public class HealthControllerSource : DataSource<HealthController> { }
}