/*
 * main.c
 * Sets up a Window object and pushes it onto the stack.
 */

#include <pebble.h>

#define WHERE_IS_XUR 5

static Window *s_main_window;
static TextLayer *s_system_layer;
static GBitmap *s_bitmap;
static BitmapLayer *s_bitmap_layer;
static char s_buffer[128];

static void main_window_load(Window *window) {  
  Layer *window_layer = window_get_root_layer(window);
  GRect bounds = layer_get_bounds(window_layer);
  
  // TEXT
  s_system_layer = text_layer_create(bounds);
  text_layer_set_background_color(s_system_layer, GColorClear);
  text_layer_set_text_alignment(s_system_layer, GTextAlignmentCenter);
  text_layer_set_text(s_system_layer, "Loading...");  
  layer_add_child(window_layer, text_layer_get_layer(s_system_layer));  
  text_layer_set_font(s_system_layer, fonts_get_system_font(FONT_KEY_GOTHIC_18));
  
  // BITMAP
  s_bitmap = gbitmap_create_with_resource(RESOURCE_ID_GUARDIANS);
  s_bitmap_layer = bitmap_layer_create(GRect(60, 138, 84, 30));
  bitmap_layer_set_bitmap(s_bitmap_layer, s_bitmap);
  bitmap_layer_set_compositing_mode(s_bitmap_layer, GCompOpSet);   
  layer_add_child(window_layer, bitmap_layer_get_layer(s_bitmap_layer));
}

static void main_window_unload(Window *window) {
  // Destroy Window's child Layers here
  text_layer_destroy(s_system_layer);
  
  bitmap_layer_destroy(s_bitmap_layer);
  gbitmap_destroy(s_bitmap);
}

static void inbox_received_callback(DictionaryIterator *iterator, void *context) {
  // Get the first pair
  Tuple *data = dict_find(iterator, WHERE_IS_XUR);
  if (data) {
    snprintf(s_buffer, sizeof(s_buffer), "%s", data->value->cstring);
    text_layer_set_text(s_system_layer, s_buffer);
  }
}

static void init() {
  app_message_register_inbox_received(inbox_received_callback);
  
  app_message_open(128, 64);
  
  // Create main Window
  s_main_window = window_create();
  window_set_window_handlers(s_main_window, (WindowHandlers) {
    .load = main_window_load,
    .unload = main_window_unload,
  });
  window_stack_push(s_main_window, true);
}

static void deinit() {
  // Destroy main Window
  window_destroy(s_main_window);
}

int main(void) {
  init();
  app_event_loop();
  deinit();
}
