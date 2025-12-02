EXTERNAL get_quest_state(quest_id)

"Xin chào! Ta là NPC A."

{get_quest_state("A") == 0:
    "Ngươi có muốn nhận nhiệm vụ đi gặp NPC B không?"
    + [Có, tôi nhận nhiệm vụ.] 
        "Tốt lắm! Hãy đến gặp NPC B và quay lại báo cáo với ta." #START_A
        -> DONE
    + [Không, để sau.]
        "Được thôi, hãy quay lại khi sẵn sàng."
        -> DONE
}

{get_quest_state("A") == 1:
    "Ngươi đã gặp NPC B chưa? Hãy đi nhanh lên!"
    -> DONE
}

{get_quest_state("A") == 2:
    "Ồ, ngươi đã gặp NPC B rồi à? Tuyệt vời!" #FINISH_A
    "Ta tặng ngươi 100 vàng làm phần thưởng!" #GIVE_REWARD_MONEY
    -> DONE
}

{get_quest_state("A") == 3:
    "Cảm ơn ngươi đã hoàn thành nhiệm vụ!"
    -> DONE
}