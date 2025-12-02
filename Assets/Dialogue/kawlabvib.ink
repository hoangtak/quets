VAR quest_a_state = 0
// 0 = chưa nhận
// 1 = đang làm (đã nói chuyện NPC B xong)
// 2 = hoàn thành
-> npc_a
=== npc_a ===

{quest_a_state:
    - 0: 
        "Xin chào, bạn có muốn nhận nhiệm vụ gặp NPC B không?"
        + "Có, tôi nhận nhiệm vụ." 
            #START_A
            "Tốt lắm. Hãy đến gặp NPC B."
            ~ quest_a_state = 1
            -> DONE
        + "Không, để sau."
            "Được thôi."
            -> DONE

    - 1:
        "Bạn đã gặp NPC B chưa?"
        -> DONE

    - 2:
        "Cảm ơn bạn đã hoàn thành nhiệm vụ!"
        -> DONE
}

=== npc_b ===

{quest_a_state == 1:
    "Tôi là NPC B. Nhiệm vụ của bạn đã hoàn thành."
    #FINISH_A
    ~ quest_a_state = 2
    -> DONE
- else:
    "Xin chào, tôi là NPC B."
    -> DONE
}
