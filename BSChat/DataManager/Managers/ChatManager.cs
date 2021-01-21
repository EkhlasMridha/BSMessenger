﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager.Contracts;
using Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataManager.Managers
{
    class ChatManager : IChatManager
    {
        private BSChatDBContext _bSChatDBContext;

        public ChatManager(BSChatDBContext bSChatDBContext)
        {
            _bSChatDBContext = bSChatDBContext;
        }

        public async Task<MessageGroup> CreateNewGroup(MessageGroup messageGroup)
        {
            messageGroup.TextMessages.Clear();
            await _bSChatDBContext.MessageGroup.AddAsync(messageGroup);
            await _bSChatDBContext.SaveChangesAsync();

            return messageGroup;
        }

        public async Task DeleteMessage(MessageGroup message)
        {
            _bSChatDBContext.MessageGroup.Remove(message);
            await _bSChatDBContext.SaveChangesAsync();
        }

        public async Task<MessageGroup> GetMessageGroup(int friendId, int ownId)
        {
            var result = await _bSChatDBContext.MessageGroup.Include(d => d.TextMessages).FirstOrDefaultAsync(s => (s.SenderId == ownId && s.ReceiverId == friendId) || (s.SenderId == friendId && s.ReceiverId == ownId));
            return result;
        }

        public async Task<ICollection<MessageGroup>> GetOldMessages(int id)
        {
            var result = await _bSChatDBContext.MessageGroup.Where(a => a.SenderId == id || a.ReceiverId == id).Include(m => m.TextMessages).ToListAsync();
            return result;
        }

        public async Task<bool> HasMessageGroup(int senderId, int receiverId)
        {
            var group = await _bSChatDBContext.MessageGroup.AnyAsync(s => (s.SenderId == senderId && s.ReceiverId == receiverId) || (s.SenderId == receiverId && s.ReceiverId == senderId));
            return group;
        }

        public async Task<TextMessage> StoreInExistingGroup(TextMessage message)
        {
            await _bSChatDBContext.TextMessage.AddAsync(message);
            await _bSChatDBContext.SaveChangesAsync();

            return message;
        }
    }
        
}
