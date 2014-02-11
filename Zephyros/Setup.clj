(defn bind [key mods f]
  (doto (Zephyros.HotKey. key mods f)
    (.Enable)))

(defn- from-rect [r]
  {:x (.X r) :y (.Y r) :w (.Width r) :h (.Height r)})

(defn get-screen-rect []
  (-> (System.Windows.Forms.Screen/PrimaryScreen)
      (.WorkingArea)
      (from-rect)))

(defn get-all-windows []
  (Zephyros.Window/GetWindows))

(defn get-window-title [win]
  (.GetTitle win))

(defn window-visible? [win]
  (.IsVisible win))

(defn get-window-rect [win]
  (from-rect (.GetRect win)))

(defn move-window [win r]
  (.Move win (:x r) (:y r) (:w r) (:h r)))
