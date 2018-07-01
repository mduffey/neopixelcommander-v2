void PrintLEDMessage(int strip, int led, int red, int green, int blue) {
    Serial.print("Strip: ");
    Serial.print(strip);
    Serial.print(". Index: ");
    Serial.print(led);
    Serial.print(". RGB: ");
    Serial.print(red);
    Serial.print(", ");
    Serial.print(green);
    Serial.print(", ");
    Serial.println(blue);
}

