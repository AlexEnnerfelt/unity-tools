using Unity.Netcode.Components;

namespace Carnage.Utils {
	public class ClientNetworkAnimator : NetworkAnimator {
		protected override bool OnIsServerAuthoritative() {
			return false;
		}
	} 
}