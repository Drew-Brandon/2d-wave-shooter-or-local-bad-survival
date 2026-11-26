extends Node

@export
var bt_plyr: BTPlayer = null

var bb: Blackboard = null

func _ready() -> void:
	bb = bt_plyr.blackboard

func set_var(var_name: StringName, value: Variant) -> void:
	bb.set_var(var_name, value)

func set_var_alt(value: Variant, var_name: StringName) -> void:
	bb.set_var(var_name, value)
