void PrintLEDUpdate(int red, int green, int blue) {
    Serial.print(F(". Colors: "));
    Serial.print(red);
    Serial.print(", ");
    Serial.print(green);
    Serial.print(", ");
    Serial.println(blue);
}
