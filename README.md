# medicine-qr-decoder
Medicine QR Decoder according the standarts in Turkey.

The standart can be found at https://goo.gl/40ht4O in Turkish.

Class Input: String of Barcode/QR
Class Output: Results Enum

Class Public Properties: Barcode, Party#, Expiry Date, Serial#

NOTE: There is a special character between serial number and expiry date of the medicine. It is read as ASCII29(F12) from any reader that is able to decode function keys. You should convert it to full-stop(".") in your code. 

NOTE: QR standart is ECC200, Barcode decoding standart is EAN-13. Readers should decode this kind of barcodes/QRs. 
