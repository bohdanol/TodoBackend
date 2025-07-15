import { useState, useEffect } from 'react';

interface TaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (taskData: any) => void;
  editTask?: any; // Task data for editing mode
  isEditMode?: boolean; // Flag to determine if it's edit or create mode
}

export default function TaskModal({ isOpen, onClose, onSave, editTask, isEditMode = false }: TaskModalProps) {
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    priority: 'Medium',
    dueDate: ''
  });

  const [isSubmitting, setIsSubmitting] = useState(false);

  // Update form data when editTask changes (for edit mode)
  useEffect(() => {
    if (isEditMode && editTask) {
      // Convert numeric priority back to string for the form
      const getPriorityString = (priority) => {
        if (typeof priority === 'number') {
          switch (priority) {
            case 0: return 'Low';
            case 1: return 'Medium';
            case 2: return 'High';
            default: return 'Medium';
          }
        }
        return priority || 'Medium';
      };

      // Format date for datetime-local input
      const formatDateForInput = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);
        // Format as YYYY-MM-DDTHH:MM for datetime-local input
        return date.toISOString().slice(0, 16);
      };

      setFormData({
        title: editTask.title || '',
        description: editTask.description || '',
        priority: getPriorityString(editTask.priority),
        dueDate: formatDateForInput(editTask.dueDate)
      });
    } else {
      // Reset form for create mode
      setFormData({
        title: '',
        description: '',
        priority: 'Medium',
        dueDate: ''
      });
    }
  }, [isEditMode, editTask, isOpen]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.title.trim()) {
      alert('Please enter a task title');
      return;
    }

    if (!formData.dueDate) {
      alert('Please select a due date');
      return;
    }

    setIsSubmitting(true);
    try {
      console.log('Form data being submitted:', formData);
      await onSave(formData);
      handleCancel();
    } catch (error) {
      console.error('Error saving task:', error);
      alert('Failed to save task. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    setFormData({
      title: '',
      description: '',
      priority: 'Medium',
      dueDate: ''
    });
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      zIndex: 1000
    }}>
      <div style={{
        backgroundColor: 'white',
        borderRadius: '8px',
        padding: '24px',
        width: '500px',
        maxWidth: '90vw',
        maxHeight: '90vh',
        overflow: 'auto',
        boxShadow: '0 4px 20px rgba(0, 0, 0, 0.15)'
      }}>
        <h2 style={{
          margin: '0 0 20px 0',
          fontSize: '1.5rem',
          color: '#2c3e50',
          borderBottom: '2px solid #ecf0f1',
          paddingBottom: '10px'
        }}>
          {isEditMode ? 'Edit Task' : 'Add New Task'}
        </h2>

        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: '16px' }}>
            <label style={{
              display: 'block',
              marginBottom: '4px',
              fontWeight: 'bold',
              color: '#2c3e50'
            }}>
              Title *
            </label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleInputChange}
              placeholder="Enter task title"
              style={{
                width: '100%',
                padding: '10px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '14px',
                boxSizing: 'border-box'
              }}
              required
            />
          </div>

          <div style={{ marginBottom: '16px' }}>
            <label style={{
              display: 'block',
              marginBottom: '4px',
              fontWeight: 'bold',
              color: '#2c3e50'
            }}>
              Description
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleInputChange}
              placeholder="Enter task description (optional)"
              rows={3}
              style={{
                width: '100%',
                padding: '10px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '14px',
                resize: 'vertical',
                boxSizing: 'border-box'
              }}
            />
          </div>

          <div style={{ marginBottom: '16px' }}>
            <label style={{
              display: 'block',
              marginBottom: '4px',
              fontWeight: 'bold',
              color: '#2c3e50'
            }}>
              Priority
            </label>
            <select
              name="priority"
              value={formData.priority}
              onChange={handleInputChange}
              style={{
                width: '100%',
                padding: '10px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '14px',
                boxSizing: 'border-box'
              }}
            >
              <option value="Low">🟢 Low</option>
              <option value="Medium">🟡 Medium</option>
              <option value="High">🔴 High</option>
            </select>
          </div>

          <div style={{ marginBottom: '24px' }}>
            <label style={{
              display: 'block',
              marginBottom: '4px',
              fontWeight: 'bold',
              color: '#2c3e50'
            }}>
              Due Date *
            </label>
            <input
              type="datetime-local"
              name="dueDate"
              value={formData.dueDate}
              onChange={handleInputChange}
              style={{
                width: '100%',
                padding: '10px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '14px',
                boxSizing: 'border-box'
              }}
              required
            />
          </div>

          <div style={{
            display: 'flex',
            gap: '12px',
            justifyContent: 'flex-end'
          }}>
            <button
              type="button"
              onClick={handleCancel}
              disabled={isSubmitting}
              style={{
                padding: '10px 20px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                backgroundColor: 'white',
                color: '#666',
                cursor: isSubmitting ? 'not-allowed' : 'pointer',
                fontSize: '14px',
                fontWeight: 'bold',
                transition: 'background-color 0.2s'
              }}
              onMouseEnter={(e) => {
                if (!isSubmitting) {
                  (e.target as HTMLElement).style.backgroundColor = '#f8f9fa';
                }
              }}
              onMouseLeave={(e) => {
                (e.target as HTMLElement).style.backgroundColor = 'white';
              }}
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              style={{
                padding: '10px 20px',
                border: 'none',
                borderRadius: '4px',
                backgroundColor: isSubmitting ? '#bdc3c7' : '#3498db',
                color: 'white',
                cursor: isSubmitting ? 'not-allowed' : 'pointer',
                fontSize: '14px',
                fontWeight: 'bold',
                transition: 'background-color 0.2s'
              }}
              onMouseEnter={(e) => {
                if (!isSubmitting) {
                  (e.target as HTMLElement).style.backgroundColor = '#2980b9';
                }
              }}
              onMouseLeave={(e) => {
                if (!isSubmitting) {
                  (e.target as HTMLElement).style.backgroundColor = '#3498db';
                }
              }}
            >
              {isSubmitting ? (isEditMode ? 'Updating...' : 'Saving...') : (isEditMode ? 'Update' : 'Save')}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
