mergeInto(LibraryManager.library, {
  SpeakText: function (textPtr, langPtr, volume, rate) {
    const text = UTF8ToString(textPtr);
    const lang = UTF8ToString(langPtr);

    if (!window.speechSynthesis) {
      console.warn("speechSynthesis not supported");
      return;
    }

    function doSpeak() {
      const voices = window.speechSynthesis.getVoices();
      if (voices.length === 0) {
        console.warn("No voices available");
        return;
      }

      // Cancelar cualquier reproducción anterior
      window.speechSynthesis.cancel();

      let preferredVoice = null;

      const isEdge = navigator.userAgent.includes("Edg");
      const isChrome = navigator.userAgent.includes("Chrome") && !isEdge;
      const isFirefox = navigator.userAgent.includes("Firefox");
      const isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);

      if (lang === "es") {
        if (isEdge) {
          preferredVoice = voices.find(v => v.name.includes("Elena") && v.lang === "es-AR");
        } else if (isChrome) {
          preferredVoice = voices.find(v => v.name.includes("Google español de Estados Unidos"));
        } else if (isFirefox || isSafari) {
          preferredVoice = voices.find(v => v.lang.startsWith("es") && v.gender === "female") || voices.find(v => v.lang.startsWith("es"));
        }
      } else if (lang === "en") {
        if (isEdge) {
          preferredVoice = voices.find(v => v.name.includes("Ava") && v.lang === "en-US");
        } else if (isChrome) {
          preferredVoice = voices.find(v => v.name.includes("Google US English"));
        } else if (isFirefox || isSafari) {
          preferredVoice = voices.find(v => v.lang.startsWith("en") && v.gender === "female") || voices.find(v => v.lang.startsWith("en"));
        }
      }

      // Fallback
      if (!preferredVoice) {
        preferredVoice = voices.find(v => v.lang.startsWith(lang)) || voices[0];
      }

      const utterance = new SpeechSynthesisUtterance(text);
      utterance.voice = preferredVoice;
      utterance.volume = volume !== undefined ? volume : 1.0;
      utterance.rate = rate !== undefined ? rate : 1.0;

      setTimeout(() => {
        window.speechSynthesis.speak(utterance);
      }, 100);
    }

    // Si las voces aún no están cargadas
    if (window.speechSynthesis.getVoices().length === 0) {
      window.speechSynthesis.onvoiceschanged = function () {
        doSpeak();
        window.speechSynthesis.onvoiceschanged = null; // limpiar
      };
      // Provocar carga de voces
      window.speechSynthesis.getVoices();
    } else {
      doSpeak();
    }
  }
});
