

# OpenUPM: WsAsymAuth

WebSocket tunnel between Unity3D and a server running asymmetric encryption for login.  
This setup allows any public-private key system to establish a connection handshake via WebSocket from Unity3D.

See [https://github.com/EloiStree/2025_01_01_APIntPushIID](https://github.com/EloiStree/2025_01_01_APIntPushIID) for server example.

### Context  

I worked for months on RSA, only to learn that ECC, which Ethereum uses, is what I should use to work with Ethereum and MetaMask.  
However, ECC, Nethereum, and Bouncy Castle are are not cross platforms: Playstation, Nintedo, Mac, Linux, UWP ...  
After losing weeks on the topic, I returned to RSA for Unity3D.  

During my exploration of ECC, I realized that I could use a coaster concept with RSA, allowing RSA to work on behalf of the ECC keys.  



### Related Repositories  

- [https://github.com/EloiStree/2021_05_29_BlockiesForUnity.git](https://github.com/EloiStree/2021_05_29_BlockiesForUnity.git)  
  - To generate blockies from the public key of Ethereum for use with the coaster.  
- [https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git](https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git)  
  - Generate and store RSA keys:  
    - [https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git](https://github.com/EloiStree/2024_04_04_GenereteRsaKeyInUnity.git)  
    - [https://github.com/EloiStree/2024_08_06_PathTypeReadWrite.git](https://github.com/EloiStree/2024_08_06_PathTypeReadWrite.git)  




