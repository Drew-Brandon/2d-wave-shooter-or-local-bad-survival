extends Node

@export var bb_vals: Dictionary[StringName, Variant]
@export var bb_nodes: Dictionary[StringName, Node]
@export var bt_plyr: BTPlayer

func _ready() -> void:
	var bb: Blackboard = bt_plyr.blackboard
	
	for val_name in bb_vals:
		bb.set_var(val_name, bb_vals[val_name])
	
	for node_name in bb_nodes:
		bb.set_var(node_name, bb_nodes[node_name])
	
	queue_free()
