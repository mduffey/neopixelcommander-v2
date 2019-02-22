void Process() {
  // Various control codes are checked on the first byte.
  // Maximum possible value for last index, highest strip is around 172, so 200, 210, etc. are control codes.
  int messageType = buffer[0];
  bool updateLEDs = false;
  switch(messageType) {
    case MESSAGE_RANGE:
      PrintMessage(F("Range"), LOG_ACTIONS);
      for (int i = 1; i < 61; i += 4) {
        if (buffer[i] == 255) {
          PrintMessage(F("End of packet."), LOG_VERBOSE);
          break;
        }
        ParsePositionAndUpdateLED(buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3]);
      }
      updateLEDs = true;
      break;
    case MESSAGE_STRIP:
      PrintMessage(F("Strip"), LOG_ACTIONS);
      UpdateLEDsForStrip(buffer[1], buffer[2], buffer[3], buffer[4]);
      updateLEDs = true;
      break;
    case MESSAGE_UNIVERSAL:
      PrintMessage(F("Universal"), LOG_ACTIONS);
      UpdateLEDs(buffer[1], buffer[2], buffer[3]);
      updateLEDs = true;
      break;
    case MESSAGE_GRADIENT:
      PrintMessage(F("Gradient (not implemented)"), LOG_ACTIONS);
      // TODO
      break;
    case MESSAGE_STATUS:
      {
        PrintMessage(F("Status"), LOG_ACTIONS);
        byte outputMessage[64];
        outputMessage[0] = MESSAGE_STATUS;
        // Max will be active, 0 will be deactivated. Will be set to handle deactivated when a potentiometer is added to the device.
        outputMessage[1] = STATUS_AVAILABLE; 
        outputMessage[2] = currentLoggingLevel;
        PrintMessage(F("Available: "), (int)STATUS_AVAILABLE, LOG_VERBOSE);
        PrintMessage(F("Logging: "), currentLoggingLevel, LOG_VERBOSE);
        RawHID.send(outputMessage, 100);
      }
      break;
    case MESSAGE_SETTINGS:
      PrintMessage(F("Settings"), LOG_ACTIONS);
      PrintMessage(F("Setting log level to: "), buffer[1], LOG_VERBOSE);
      currentLoggingLevel = buffer[1];
      break;
    default:
      PrintMessage(F("Unsupported: "), buffer[0], LOG_UNEXPECTED);
      break;
  }
  if (updateLEDs) {
    FastLED.show();
  }
}

