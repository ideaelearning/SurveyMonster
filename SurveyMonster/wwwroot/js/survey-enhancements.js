/**
 * Survey Enhancements JavaScript
 * Handles animations and interactive behaviors for survey features
 */

(function() {
    'use strict';

    // ============================================
    // Question Navigation Animations
    // ============================================

    /**
     * Animates question transitions when navigating forward/backward
     */
    function initQuestionTransitions() {
        const questionForm = document.getElementById('questionForm');
        if (!questionForm) return;

        const questionCard = document.querySelector('.question-card');
        if (!questionCard) return;

        // Store the current direction for animation
        let navigationDirection = 'forward';

        // Listen for navigation button clicks
        const nextBtn = document.getElementById('nextBtn');
        const prevBtns = document.querySelectorAll('button[name="nextIndex"]');

        if (nextBtn) {
            nextBtn.addEventListener('click', function() {
                navigationDirection = 'forward';
                animateQuestionExit(questionCard, navigationDirection);
            });
        }

        prevBtns.forEach(btn => {
            btn.addEventListener('click', function() {
                const nextIndex = parseInt(this.value);
                const currentIndex = parseInt(document.querySelector('input[name="entryId"]')?.closest('form')?.querySelector('.question-number')?.textContent || 0);
                navigationDirection = nextIndex < currentIndex ? 'backward' : 'forward';
                animateQuestionExit(questionCard, navigationDirection);
            });
        });

        // Animate question entrance on page load
        if (questionCard) {
            questionCard.classList.add('slide-in-right');
        }
    }

    /**
     * Animates question exit based on navigation direction
     */
    function animateQuestionExit(element, direction) {
        if (direction === 'forward') {
            element.classList.add('slide-out-left');
        } else {
            element.classList.add('slide-out-right');
        }
    }

    // ============================================
    // Progress Bar Animation
    // ============================================

    /**
     * Animates progress bar updates
     */
    function initProgressBarAnimation() {
        const progressFill = document.getElementById('progressFill');
        if (!progressFill) return;

        // Add animate class on load
        progressFill.classList.add('animate');

        // Update progress with animation when answers change
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'attributes' && mutation.attributeName === 'style') {
                    progressFill.classList.remove('animate');
                    setTimeout(() => {
                        progressFill.classList.add('animate');
                    }, 10);
                }
            });
        });

        observer.observe(progressFill, {
            attributes: true,
            attributeFilter: ['style']
        });
    }

    // ============================================
    // Button Hover Effects
    // ============================================

    /**
     * Enhances button interactions with ripple effects
     */
    function initButtonEffects() {
        const buttons = document.querySelectorAll('.start-btn, .submit-btn, .cancel-btn');
        
        buttons.forEach(button => {
            button.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-3px)';
            });

            button.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
            });

            // Add ripple effect on click
            button.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
                ripple.classList.add('ripple-effect');
                
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;
                
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = x + 'px';
                ripple.style.top = y + 'px';
                
                this.appendChild(ripple);
                
                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        });
    }

    // ============================================
    // Option Selection Animation
    // ============================================

    /**
     * Animates option selection for radio buttons and checkboxes
     */
    function initOptionSelectionAnimation() {
        const optionInputs = document.querySelectorAll('.option-item input');
        
        optionInputs.forEach(input => {
            input.addEventListener('change', function() {
                const optionItem = this.closest('.option-item');
                if (!optionItem) return;

                // Remove selected class from all options if radio button
                if (this.type === 'radio') {
                    document.querySelectorAll('.option-item').forEach(item => {
                        item.classList.remove('selected');
                    });
                }

                // Add selected class and animation
                if (this.checked) {
                    optionItem.classList.add('selected');
                    
                    // Trigger selection animation
                    optionItem.style.animation = 'none';
                    setTimeout(() => {
                        optionItem.style.animation = '';
                    }, 10);
                } else {
                    optionItem.classList.remove('selected');
                }
            });

            // Initialize selected state on page load
            if (input.checked) {
                const optionItem = input.closest('.option-item');
                if (optionItem) {
                    optionItem.classList.add('selected');
                }
            }
        });
    }

    // ============================================
    // Consent Checkbox Animation
    // ============================================

    /**
     * Enhances consent checkbox interaction
     */
    function initConsentAnimation() {
        const consentCheckbox = document.getElementById('consentCheckbox');
        const startBtn = document.getElementById('startSurveyBtn');
        
        if (!consentCheckbox || !startBtn) return;

        consentCheckbox.addEventListener('change', function() {
            startBtn.disabled = !this.checked;
            
            // Add/remove pulse animation
            if (this.checked) {
                startBtn.classList.add('pulse');
                
                // Trigger a success animation
                this.style.animation = 'checkBounce 0.3s ease-out';
                setTimeout(() => {
                    this.style.animation = '';
                }, 300);
            } else {
                startBtn.classList.remove('pulse');
            }
        });
    }

    // ============================================
    // Preview Modal Animation
    // ============================================

    /**
     * Animates preview modal entrance
     */
    function initPreviewModalAnimation() {
        const previewBtn = document.getElementById('previewBtn');
        const previewModal = document.getElementById('previewModal');
        
        if (!previewBtn || !previewModal) return;

        previewModal.addEventListener('show.bs.modal', function() {
            // Stagger animation for preview questions will be handled by CSS
            const questions = this.querySelectorAll('.preview-question');
            questions.forEach((question, index) => {
                question.style.animationDelay = (index * 0.1) + 's';
            });
        });
    }

    // ============================================
    // Form Validation Animation
    // ============================================

    /**
     * Animates form validation errors
     */
    function initFormValidationAnimation() {
        const forms = document.querySelectorAll('form');
        
        forms.forEach(form => {
            form.addEventListener('submit', function(e) {
                const invalidInputs = this.querySelectorAll('.is-invalid');
                
                invalidInputs.forEach(input => {
                    // Trigger shake animation
                    input.style.animation = 'shake 0.5s ease-out';
                    setTimeout(() => {
                        input.style.animation = '';
                    }, 500);
                });
            });
        });

        // Real-time validation animation
        const inputs = document.querySelectorAll('.form-control');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                if (this.classList.contains('is-invalid')) {
                    this.style.animation = 'shake 0.5s ease-out';
                    setTimeout(() => {
                        this.style.animation = '';
                    }, 500);
                }
            });
        });
    }

    // ============================================
    // Smooth Scroll Enhancement
    // ============================================

    /**
     * Enhances smooth scrolling for navigation
     */
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function(e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    // ============================================
    // Loading State Animation
    // ============================================

    /**
     * Shows loading state on form submission
     */
    function initLoadingStateAnimation() {
        const forms = document.querySelectorAll('form');
        
        forms.forEach(form => {
            form.addEventListener('submit', function() {
                const submitBtn = this.querySelector('button[type="submit"]');
                if (submitBtn && !submitBtn.hasAttribute('formnovalidate')) {
                    submitBtn.classList.add('loading');
                    submitBtn.disabled = true;
                    
                    // Store original text
                    const originalText = submitBtn.innerHTML;
                    submitBtn.innerHTML = '<i class="bi bi-hourglass-split me-2"></i>İşleniyor...';
                    
                    // Restore if form validation fails
                    setTimeout(() => {
                        if (this.querySelector('.is-invalid')) {
                            submitBtn.classList.remove('loading');
                            submitBtn.disabled = false;
                            submitBtn.innerHTML = originalText;
                        }
                    }, 100);
                }
            });
        });
    }

    // ============================================
    // Textarea Auto-expand
    // ============================================

    /**
     * Auto-expands textarea as user types
     */
    function initTextareaAutoExpand() {
        const textareas = document.querySelectorAll('textarea.form-control');
        
        textareas.forEach(textarea => {
            textarea.addEventListener('input', function() {
                this.style.height = 'auto';
                this.style.height = (this.scrollHeight) + 'px';
            });

            // Initialize height on page load if there's content
            if (textarea.value) {
                textarea.style.height = 'auto';
                textarea.style.height = (textarea.scrollHeight) + 'px';
            }
        });
    }

    // ============================================
    // Info Card Stagger Animation
    // ============================================

    /**
     * Staggers info card entrance animations
     */
    function initInfoCardAnimation() {
        const infoItems = document.querySelectorAll('.info-item');
        
        infoItems.forEach((item, index) => {
            item.style.animationDelay = (index * 0.1) + 's';
        });
    }

    // ============================================
    // Reduced Motion Support
    // ============================================

    /**
     * Respects user's reduced motion preference
     */
    function checkReducedMotion() {
        const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
        
        if (prefersReducedMotion) {
            document.documentElement.style.setProperty('--animation-duration', '0.01ms');
            document.documentElement.style.setProperty('--transition-duration', '0.01ms');
        }
    }

    // ============================================
    // Initialize All Enhancements
    // ============================================

    /**
     * Initialize all survey enhancements when DOM is ready
     */
    function init() {
        // Check for reduced motion preference
        checkReducedMotion();

        // Initialize all features
        initQuestionTransitions();
        initProgressBarAnimation();
        initButtonEffects();
        initOptionSelectionAnimation();
        initConsentAnimation();
        initPreviewModalAnimation();
        initFormValidationAnimation();
        initSmoothScroll();
        initLoadingStateAnimation();
        initTextareaAutoExpand();
        initInfoCardAnimation();
    }

    // Run initialization when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
