EXTERNAL get_quest_state(quest_id)

"Xin chào! Ta là NPC B."

{get_quest_state("A") == 1:
    "À, ngươi được NPC A sai đến đúng không?"
    "Ta đã nhận được tin nhắn rồi!" #PROGRESS_A
    "Hãy quay lại báo cáo với NPC A nhé!"
    -> DONE
- else:
    "Có việc gì ta giúp được ngươi không?"
    -> DONE
}