#if NETCODE
using Unity.Netcode.Components;
#endif
namespace Carnage.Utils {
#if NETCODE
	public class ClientNetworkAnimator : NetworkAnimator {
		protected override bool OnIsServerAuthoritative() {
			return false;
		}
	}
#endif
}