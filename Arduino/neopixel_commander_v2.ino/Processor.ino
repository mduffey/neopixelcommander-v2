#define RANGE 10  // Specific LEDs
#define STRIP 20 // All LEDs on one strip
#define UNIVERSAL 30 // All strips, all LEDs
#define GRADIENT 40 // Specifies two colors, and the gradient will be managed by the Arduino
#define SETTINGS 200 // Updates settings. For now, alters log frequency.
#define LOG_VERBOSE 0
#define LOG_ACTIONS 10
#define LOG_UNEXPECTED 20
#define LOG_NONE 30

void Process() {
  // Various control codes are checked on the first byte.
  // Maximum possible value for last index, highest strip is around 172, so 200, 210, etc. are control codes.
  if (buffer[0] == RANGE) {
    PrintMessage(F("Range"), LOG_ACTIONS);
    for (int i = 1; i < 61; i += 4) {
      if (buffer[i] == 255) {
        PrintMessage(F("End of packet."), LOG_VERBOSE);
        break;
      }
      ParsePositionAndUpdateLED(buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3]);
    }
  }
  else if (buffer[0] == STRIP) {
    PrintMessage(F("Strip"), LOG_ACTIONS);
    int strip = buffer[1];
    UpdateLEDsForStrip(strip, buffer[2], buffer[3], buffer[4]);
  }
  else if (buffer[0] == UNIVERSAL) {
    PrintMessage(F("Universal"), LOG_ACTIONS);
    UpdateLEDs(buffer[1], buffer[2], buffer[3]);
  }
  else if (buffer[0] == GRADIENT) {
    PrintMessage(F("Gradient"), LOG_ACTIONS);
    UpdateLEDs(0, 0, 0);
  } 
  else if (buffer[0] == SETTINGS) {
    currentLoggingLevel = buffer[1];
  }
  else {
    PrintMessageAndValue(F("Unsupported: "), buffer[0], LOG_UNEXPECTED);
  }
  FastLED.show();
}

