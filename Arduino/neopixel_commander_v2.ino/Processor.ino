#define RESET 200
#define RESTART 210
#define STATUS 220

void Process() {
  // Various control codes are checked on the first byte.
  // Maximum possible value for last index, highest strip is around 172, so 200, 210, etc. are control codes.
  if (buffer[0] == RESET) { // reset strip
    for(int strip = 0; strip < NUM_STRIPS; strip++) {
      for (int i = 0; i < NUM_LEDS_PER_STRIP; i++) {
        int pos = i << 2;
        pos = pos | strip;
        UpdateLED(pos, 0, 0, 0, false);
      }
    }
  }
  else if (buffer[0] == RESTART) {
    
  }
  else if (buffer[0] == STATUS) { // Get status
  
  }
  else {
    for (int i = 0; i < 64; i += 4) {
      if (buffer[i] == 255) {
        break;
      }
      UpdateLED(buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3], true);
    }
  }
  FastLED.show();
  
}

void UpdateLED(int pos, int red, int green, int blue, bool print) {
  int strip = pos & 3;
  int led = pos >> 2;
  leds[strip][led].setRGB(red, green, blue);
}
