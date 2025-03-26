

# OpenUPM: WsAsymAuth

WebSocket tunnel between Unity3D and a server running asymmetric encryption for login.  
This setup allows any public-private key system to establish a connection handshake via WebSocket from Unity3D.

See [https://github.com/EloiStree/2025_01_01_APIntPushIID](https://github.com/EloiStree/2025_01_01_APIntPushIID) for server example.

### Context  

I worked for months on RSA, only to learn that ECC, which Ethereum uses, is what I should use to work with Ethereum and MetaMask.  
However, ECC, Nethereum, and Bouncy Castle are are not cross platforms: Playstation, Nintedo, Mac, Linux, UWP ...  
After losing weeks on the topic, I returned to RSA for Unity3D.  

During my exploration of ECC, I realized that I could use a coaster concept with RSA, allowing RSA to work on behalf of the ECC keys.  


### See Also  

- [https://github.com/EloiStree/2025_01_01_CreateSignVerifyWithNethereum.git](https://github.com/EloiStree/2025_01_01_CreateSignVerifyWithNethereum.git)  
  - A working Nethereum code for Windows.  
  - With [https://github.com/EloiStree/2025_01_01_UnityServerTunnelingMetaMaskUnity.git](https://github.com/EloiStree/2025_01_01_UnityServerTunnelingMetaMaskUnity.git)  
    - A tunnel that establishes a WebSocket connection with a server using ECC.  

This package is designed to be used in the context of this tunneling package:  
- [https://github.com/EloiStree/2024_04_04_UnityServerTunnelingRSAUnity](https://github.com/EloiStree/2024_04_04_UnityServerTunnelingRSAUnity)  

### Related Repositories  

- [https://github.com/EloiStree/2021_05_29_BlockiesForUnity.git](https://github.com/EloiStree/2021_05_29_BlockiesForUnity.git)  
  - To generate blockies from the public key of Ethereum for use with the coaster.  
- [https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git](https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git)  
  - Generate and store RSA keys:  
    - [https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git](https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git)  
    - [https://github.com/EloiStree/2024_08_06_PathTypeReadWrite.git](https://github.com/EloiStree/2024_08_06_PathTypeReadWrite.git)  


 
---------------
**In short:**  
The goal here is to be able to communicated between library in ETH and Unity in RSA with WebSocket server:
- https://github.com/EloiStree/OpenUPM_pBit4096B58Pkcs1SHA256
  - https://github.com/EloiStree/OpenUPM_AsymmetricalClipboardCoaster
    - https://github.com/EloiStree/OpenUPM_WsMetaMaskAuth
- https://github.com/EloiStree/OpenUPM_IID.git
