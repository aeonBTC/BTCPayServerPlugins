document.addEventListener('DOMContentLoaded', function() {
    // Add button to add new shipping threshold row
    const addShippingThresholdBtn = document.createElement('button');
    addShippingThresholdBtn.type = 'button';
    addShippingThresholdBtn.className = 'btn btn-secondary btn-sm mt-2';
    addShippingThresholdBtn.textContent = 'Add Shipping Threshold';
    
    const shippingTable = document.querySelector('table:nth-of-type(1)');
    shippingTable.parentNode.insertBefore(addShippingThresholdBtn, shippingTable.nextSibling);
    
    addShippingThresholdBtn.addEventListener('click', function() {
        const tbody = shippingTable.querySelector('tbody');
        const lastRow = tbody.lastElementChild;
        const newRow = lastRow.cloneNode(true);
        
        // Clear input values
        newRow.querySelectorAll('input').forEach(input => {
            input.value = '';
            // Update input name/id to reflect new index
            const currentIndex = parseInt(input.name.match(/\d+/)[0]);
            const newIndex = currentIndex + 1;
            input.name = input.name.replace(`[${currentIndex}]`, `[${newIndex}]`);
            input.id = input.id.replace(`_${currentIndex}__`, `_${newIndex}__`);
        });
        
        tbody.appendChild(newRow);
    });
    
    // Add button to add new discount threshold row
    const addDiscountThresholdBtn = document.createElement('button');
    addDiscountThresholdBtn.type = 'button';
    addDiscountThresholdBtn.className = 'btn btn-secondary btn-sm mt-2';
    addDiscountThresholdBtn.textContent = 'Add Discount Threshold';
    
    const discountTable = document.querySelector('table:nth-of-type(2)');
    discountTable.parentNode.insertBefore(addDiscountThresholdBtn, discountTable.nextSibling);
    
    addDiscountThresholdBtn.addEventListener('click', function() {
        const tbody = discountTable.querySelector('tbody');
        const lastRow = tbody.lastElementChild;
        const newRow = lastRow.cloneNode(true);
        
        // Clear input values
        newRow.querySelectorAll('input').forEach(input => {
            input.value = '';
            // Update input name/id to reflect new index
            const currentIndex = parseInt(input.name.match(/\d+/)[0]);
            const newIndex = currentIndex + 1;
            input.name = input.name.replace(`[${currentIndex}]`, `[${newIndex}]`);
            input.id = input.id.replace(`_${currentIndex}__`, `_${newIndex}__`);
        });
        
        tbody.appendChild(newRow);
    });
}); 