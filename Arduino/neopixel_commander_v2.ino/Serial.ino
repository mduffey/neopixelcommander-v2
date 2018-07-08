void PrintMessage(const __FlashStringHelper* message, int logLevel) {
  if (currentLoggingLevel <= logLevel) {
    Serial.println(message);
  }
}

void PrintMessage(const __FlashStringHelper* message, int value, int logLevel) {
  if (currentLoggingLevel <= logLevel) {
    Serial.print(message);
    Serial.println(value);
  }
}

void PrintLEDUpdate(int strip, int led, int red, int green, int blue) {
  if (currentLoggingLevel == LOG_VERBOSE) {
    Serial.print(F("Strip: "));
    Serial.print(strip);
    Serial.print(F(". LED: "));
    Serial.print(led);
    Serial.print(F(". Colors: "));
    Serial.print(red);
    Serial.print(", ");
    Serial.print(green);
    Serial.print(", ");
    Serial.println(blue);
  }
}
