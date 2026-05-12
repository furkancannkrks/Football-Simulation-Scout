document.addEventListener('DOMContentLoaded', () => {
    // Elements
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabPanes = document.querySelectorAll('.tab-pane');
    const scoutForm = document.getElementById('scoutForm');
    const ratingsTbody = document.getElementById('ratings-tbody');
    const scoutResults = document.getElementById('scout-results');

    // Pagination State
    let scoutedPlayers = [];
    let currentPage = 1;
    const itemsPerPage = 12;

    // Toast System
    function showToast(message, type = 'info') {
        const container = document.getElementById('toast-container');
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        
        let icon = 'ℹ️';
        if (type === 'success') icon = '✅';
        if (type === 'error') icon = '❌';
        
        toast.innerHTML = `<span>${icon}</span> <span>${message}</span>`;
        container.appendChild(toast);
        
        setTimeout(() => toast.classList.add('show'), 10);
        
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // Tab Switching Logic
    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            tabBtns.forEach(b => b.classList.remove('active'));
            tabPanes.forEach(p => p.classList.remove('active'));

            btn.classList.add('active');
            const targetId = btn.getAttribute('data-target');
            document.getElementById(targetId).classList.add('active');
        });
    });

    // Fetch and populate Top 20 Ratings
    async function loadRatings() {
        try {
            const response = await fetch('/api/players/ratings');
            if (!response.ok) throw new Error('Network response was not ok');
            
            const players = await response.json();
            
            if (players.length === 0) {
                ratingsTbody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: var(--text-muted);">No rating data available yet.</td></tr>`;
                return;
            }

            let html = '';
            players.forEach((player, index) => {
                html += `
                    <tr>
                        <td>#${index + 1}</td>
                        <td style="font-weight: 500;">${player.name}</td>
                        <td style="color: var(--text-muted);">${player.team}</td>
                        <td><span class="rating-badge">${player.rating.toFixed(1)}</span></td>
                    </tr>
                `;
            });
            ratingsTbody.innerHTML = html;
        } catch (error) {
            console.error('Error fetching ratings:', error);
            ratingsTbody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: #ef4444;">An error occurred while loading ratings.</td></tr>`;
            showToast('Failed to load Top 20 Ratings.', 'error');
        }
    }

    // ------------------------------------------------------------------------
    // Team Stats Logic
    // ------------------------------------------------------------------------
    let teamStatsData = [];
    let currentSort = { column: 'totalGoals', asc: false };

    async function loadTeamStats() {
        try {
            const response = await fetch('/api/players/team-stats');
            if (!response.ok) throw new Error('Failed to load team stats');
            
            teamStatsData = await response.json();
            renderTeamStats();
        } catch (error) {
            console.error(error);
            showToast('Failed to load Team Stats.', 'error');
        }
    }

    function renderTeamStats() {
        const tbody = document.getElementById('teams-tbody');
        if (teamStatsData.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-muted);">No team data available.</td></tr>`;
            return;
        }

        teamStatsData.sort((a, b) => {
            let valA = a[currentSort.column];
            let valB = b[currentSort.column];
            
            if(typeof valA === 'string') valA = valA.toLowerCase();
            if(typeof valB === 'string') valB = valB.toLowerCase();

            if (valA < valB) return currentSort.asc ? -1 : 1;
            if (valA > valB) return currentSort.asc ? 1 : -1;
            return 0;
        });

        let html = '';
        teamStatsData.forEach((team, index) => {
            const highlightClass = index === 0 ? 'best-team' : '';
            html += `
                <tr class="${highlightClass}">
                    <td style="font-weight: 500;">${team.team}</td>
                    <td>${team.totalGoals}</td>
                    <td>${team.totalAssists}</td>
                    <td>${team.avgXg.toFixed(2)}</td>
                    <td>${team.playerCount}</td>
                </tr>
            `;
        });
        tbody.innerHTML = html;
    }

    document.querySelectorAll('.sortable-table th').forEach(th => {
        th.addEventListener('click', () => {
            const column = th.getAttribute('data-sort');
            if (currentSort.column === column) {
                currentSort.asc = !currentSort.asc;
            } else {
                currentSort.column = column;
                currentSort.asc = false;
            }
            renderTeamStats();
        });
    });

    // Handle Scout Form Submission
    scoutForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const submitBtn = document.getElementById('scoutSubmitBtn');
        const btnText = submitBtn.querySelector('.btn-text');
        const btnSpinner = submitBtn.querySelector('.btn-spinner');
        
        submitBtn.disabled = true;
        btnSpinner.classList.remove('hidden');
        btnText.textContent = 'Scouting...';

        const formData = new FormData(scoutForm);
        const params = new URLSearchParams();
        
        for (const [key, value] of formData.entries()) {
            if (value && value.trim() !== '') {
                params.append(key, value.trim());
            }
        }
        
        scoutResults.innerHTML = '';
        document.querySelector('.tab-btn[data-target="tab-scout"]').click();

        try {
            const response = await fetch(`/api/players/scout?${params.toString()}`);
            if (!response.ok) throw new Error('Network response was not ok');
            
            scoutedPlayers = await response.json();
            
            if (scoutedPlayers.length === 0) {
                scoutResults.innerHTML = `
                    <div class="empty-state">
                        <p>Bu filtrelere uygun oyuncu bulunamadı.</p>
                        <p class="sub-text">Farklı filtreler deneyebilirsiniz.</p>
                    </div>
                `;
                document.getElementById('pagination-controls').classList.add('hidden');
                showToast('Bu filtrelere uygun oyuncu bulunamadı.', 'info');
            } else {
                renderPage(1);
                showToast(`Found ${scoutedPlayers.length} players!`, 'success');
            }
        } catch (error) {
            console.error('Error fetching scout results:', error);
            scoutResults.innerHTML = `
                <div class="empty-state" style="border-color: #ef4444;">
                    <p style="color: #ef4444;">An error occurred while fetching results.</p>
                </div>
            `;
            document.getElementById('pagination-controls').classList.add('hidden');
            showToast('Error fetching scout results.', 'error');
        } finally {
            submitBtn.disabled = false;
            btnSpinner.classList.add('hidden');
            btnText.textContent = 'Scout Players';
        }
    });

    // Pagination Logic
    function renderPage(page) {
        currentPage = page;
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        const pageData = scoutedPlayers.slice(startIndex, endIndex);

        let html = '';
        pageData.forEach(player => {
            html += `
                <div class="player-card">
                    <div class="card-header-toggle" onclick="this.parentElement.classList.toggle('expanded')">
                        <div class="card-basic-info">
                            <h3>${player.name || 'Unknown'}</h3>
                            <p>${player.team || 'Free Agent'}</p>
                            <span class="position-badge">${player.position || 'N/A'}</span>
                        </div>
                        <div class="card-toggle-icon">▼</div>
                    </div>
                    <div class="card-details-wrapper">
                        <div class="card-details-inner">
                            <div class="card-stats-grid">
                                <div class="stat-item"><span class="stat-label">Appearances</span><span class="stat-value">${player.appearances || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Minutes Played</span><span class="stat-value">${player.minutesPlayed || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Goals</span><span class="stat-value">${player.goals || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Assists</span><span class="stat-value">${player.assists || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">xG</span><span class="stat-value">${player.xg || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Key Passes</span><span class="stat-value">${player.keyPasses || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Accurate Passes</span><span class="stat-value">${player.accuratePasses || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Tackles</span><span class="stat-value">${player.tackles || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Interceptions</span><span class="stat-value">${player.interceptions || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Clearances</span><span class="stat-value">${player.clearances || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Aerials Won</span><span class="stat-value">${player.aerialsWon || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Duels Won</span><span class="stat-value">${player.duelsWon || 0}</span></div>
                                <div class="stat-item"><span class="stat-label">Saves</span><span class="stat-value">${player.saves || 0}</span></div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        });
        
        scoutResults.innerHTML = html;
        renderPaginationControls();
        
        document.getElementById('tab-scout').scrollTo({ top: 0, behavior: 'smooth' });
    }

    function renderPaginationControls() {
        const totalPages = Math.ceil(scoutedPlayers.length / itemsPerPage);
        const paginationContainer = document.getElementById('pagination-controls');
        
        if (totalPages <= 1) {
            paginationContainer.classList.add('hidden');
            return;
        }
        
        paginationContainer.classList.remove('hidden');
        let html = '';

        html += `<button class="page-btn" ${currentPage === 1 ? 'disabled' : ''} onclick="window.changePage(${currentPage - 1})">&lt;</button>`;

        for (let i = 1; i <= totalPages; i++) {
            if(i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
                html += `<button class="page-btn ${i === currentPage ? 'active' : ''}" onclick="window.changePage(${i})">${i}</button>`;
            } else if (i === currentPage - 3 || i === currentPage + 3) {
                html += `<span style="color: var(--text-muted); padding: 0.5rem;">...</span>`;
            }
        }

        html += `<button class="page-btn" ${currentPage === totalPages ? 'disabled' : ''} onclick="window.changePage(${currentPage + 1})">&gt;</button>`;

        paginationContainer.innerHTML = html;
    }

    window.changePage = function(page) {
        renderPage(page);
    };

    // ------------------------------------------------------------------------
    // Match Simulation & Autocomplete Logic
    // ------------------------------------------------------------------------
    let allPlayers = [];
    
    async function loadAllBasicPlayers() {
        try {
            const response = await fetch('/api/players/all-basic');
            if (!response.ok) throw new Error('Failed to load basic players');
            allPlayers = await response.json();
            
            const teams = [...new Set(allPlayers.map(p => p.team))].filter(t => t).sort();
            const homeSelect = document.getElementById('homeTeamSelect');
            const awaySelect = document.getElementById('awayTeamSelect');
            const manageSelect = document.getElementById('manageTeamSelect');
            
            teams.forEach(team => {
                homeSelect.add(new Option(team, team));
                awaySelect.add(new Option(team, team));
                manageSelect.add(new Option(team, team));
            });

            const datalist = document.getElementById('player-names-list');
            allPlayers.forEach(p => {
                if(p.name) {
                    const option = document.createElement('option');
                    option.value = p.name;
                    datalist.appendChild(option);
                }
            });

        } catch (err) {
            console.error(err);
            showToast('Failed to load basic player data.', 'error');
        }
    }

    // ------------------------------------------------------------------------
    // Interactive Pitch Builder Logic
    // ------------------------------------------------------------------------
    let pitchSquads = {
        home: new Array(11).fill(null),
        away: new Array(11).fill(null)
    };
    
    let currentFormation = '4-3-3';

    const formationConfigs = {
        '4-3-3': {
            labels: ['GK', 'LB', 'CB', 'CB', 'RB', 'LM', 'CM', 'RM', 'LW', 'ST', 'RW'],
            positions: [
                { left: '50%', top: '90%' },
                { left: '15%', top: '72%' },
                { left: '35%', top: '75%' },
                { left: '65%', top: '75%' },
                { left: '85%', top: '72%' },
                { left: '20%', top: '48%' },
                { left: '50%', top: '50%' },
                { left: '80%', top: '48%' },
                { left: '20%', top: '20%' },
                { left: '50%', top: '15%' },
                { left: '80%', top: '20%' }
            ]
        },
        '4-4-2': {
            labels: ['GK', 'LB', 'CB', 'CB', 'RB', 'LM', 'CM', 'CM', 'RM', 'ST', 'ST'],
            positions: [
                { left: '50%', top: '90%' },
                { left: '15%', top: '72%' },
                { left: '35%', top: '75%' },
                { left: '65%', top: '75%' },
                { left: '85%', top: '72%' },
                { left: '15%', top: '46%' },
                { left: '38%', top: '48%' },
                { left: '62%', top: '48%' },
                { left: '85%', top: '46%' },
                { left: '35%', top: '18%' },
                { left: '65%', top: '18%' }
            ]
        },
        '3-5-2': {
            labels: ['GK', 'CB', 'CB', 'CB', 'LWB', 'DM', 'DM', 'RWB', 'AM', 'ST', 'ST'],
            positions: [
                { left: '50%', top: '90%' },
                { left: '20%', top: '75%' },
                { left: '50%', top: '76%' },
                { left: '80%', top: '75%' },
                { left: '12%', top: '52%' },
                { left: '35%', top: '58%' },
                { left: '65%', top: '58%' },
                { left: '88%', top: '52%' },
                { left: '50%', top: '38%' },
                { left: '35%', top: '16%' },
                { left: '65%', top: '16%' }
            ]
        },
        '5-3-2': {
            labels: ['GK', 'LWB', 'CB', 'CB', 'CB', 'RWB', 'CM', 'CM', 'CM', 'ST', 'ST'],
            positions: [
                { left: '50%', top: '90%' },
                { left: '10%', top: '68%' },
                { left: '28%', top: '75%' },
                { left: '50%', top: '76%' },
                { left: '72%', top: '75%' },
                { left: '90%', top: '68%' },
                { left: '25%', top: '46%' },
                { left: '50%', top: '48%' },
                { left: '75%', top: '46%' },
                { left: '35%', top: '16%' },
                { left: '65%', top: '16%' }
            ]
        }
    };

    window.switchSimTab = function(teamPrefix) {
        document.getElementById('btn-subtab-home').classList.remove('active');
        document.getElementById('btn-subtab-away').classList.remove('active');
        document.getElementById('btn-subtab-' + teamPrefix).classList.add('active');
        
        document.getElementById('viewport-home').classList.add('hidden');
        document.getElementById('viewport-away').classList.add('hidden');
        document.getElementById('viewport-' + teamPrefix).classList.remove('hidden');
        
        document.getElementById('btn-subtab-home').style.borderColor = teamPrefix === 'home' ? 'var(--primary)' : 'var(--border)';
        document.getElementById('btn-subtab-away').style.borderColor = teamPrefix === 'away' ? 'var(--primary)' : 'var(--border)';
    };

    window.changeFormation = function(formation) {
        currentFormation = formation;
        const config = formationConfigs[formation];
        if (!config) return;

        ['home', 'away'].forEach(teamPrefix => {
            const pitch = document.getElementById('pitch-' + teamPrefix);
            if (!pitch) return;
            pitch.dataset.formation = formation;
            
            const slots = pitch.querySelectorAll('.pitch-slot');
            slots.forEach((slot) => {
                const id = parseInt(slot.getAttribute('data-slot-id'), 10);
                if (isNaN(id)) return;
                
                slot.style.left = config.positions[id].left;
                slot.style.top = config.positions[id].top;
                
                if (!slot.classList.contains('occupied')) {
                    const labelSpan = slot.querySelector('.slot-label');
                    if (labelSpan) {
                        labelSpan.textContent = config.labels[id];
                    }
                }
            });
        });
        
        updatePitchSlotsDisplay('home');
        updatePitchSlotsDisplay('away');
    };

    function filterPitchPlayers(searchInputId, containerId) {
        const query = document.getElementById(searchInputId).value.toLowerCase();
        const container = document.getElementById(containerId);
        if (!container) return;
        const cards = container.querySelectorAll('.draggable-player-card');
        
        cards.forEach(card => {
            const text = card.textContent.toLowerCase();
            if (text.includes(query)) {
                card.style.display = '';
            } else {
                card.style.display = 'none';
            }
        });
    }

    document.getElementById('homePlayerSearch').addEventListener('input', () => filterPitchPlayers('homePlayerSearch', 'homeAvailablePlayersList'));
    document.getElementById('awayPlayerSearch').addEventListener('input', () => filterPitchPlayers('awayPlayerSearch', 'awayAvailablePlayersList'));

    function renderAvailablePlayersForPitch(teamName, teamPrefix) {
        const container = document.getElementById(teamPrefix + 'AvailablePlayersList');
        const searchInput = document.getElementById(teamPrefix + 'PlayerSearch');
        if (!container) return;
        container.innerHTML = '';
        if (searchInput) searchInput.value = '';
        
        pitchSquads[teamPrefix].fill(null);
        updatePitchSlotsDisplay(teamPrefix);
        
        if (!teamName) {
            container.innerHTML = '<p class="sub-text" style="text-align: center; margin-top: 2rem;">Select a team first</p>';
            if (searchInput) searchInput.classList.add('hidden');
            return;
        }
        
        if (searchInput) searchInput.classList.remove('hidden');
        const teamPlayers = allPlayers.filter(p => p.team === teamName).sort((a, b) => a.name.localeCompare(b.name));
        
        if (teamPlayers.length === 0) {
            container.innerHTML = '<p class="sub-text" style="text-align: center; margin-top: 2rem;">No players found for this team</p>';
            return;
        }

        teamPlayers.forEach(player => {
            const card = document.createElement('div');
            card.className = 'draggable-player-card';
            card.draggable = true;
            card.dataset.name = player.name;
            card.dataset.position = player.position || 'N/A';
            card.id = `dragcard-${teamPrefix}-${player.name.replace(/\s+/g, '-')}`;
            
            card.innerHTML = `
                <div style="display: flex; align-items: center; gap: 0.75rem;">
                    <span class="position-badge" style="padding: 0.15rem 0.5rem; font-size: 0.75rem;">${player.position || 'N/A'}</span>
                    <span style="font-weight: 600; font-size: 0.9rem;">${player.name}</span>
                </div>
                <span style="color: var(--text-muted); font-size: 1.1rem; pointer-events: none;">≡</span>
            `;
            
            card.addEventListener('dragstart', (e) => {
                if (card.classList.contains('placed')) {
                    e.preventDefault();
                    return;
                }
                e.dataTransfer.setData('text/plain', JSON.stringify({
                    name: player.name,
                    position: player.position,
                    teamPrefix: teamPrefix
                }));
                e.dataTransfer.effectAllowed = 'move';
                setTimeout(() => card.style.opacity = '0.4', 0);
            });
            
            card.addEventListener('dragend', () => {
                card.style.opacity = '';
            });
            
            container.appendChild(card);
        });
    }

    function setupPitchSlots() {
        const slots = document.querySelectorAll('.pitch-slot');
        slots.forEach(slot => {
            const teamPrefix = slot.getAttribute('data-team');
            const slotId = parseInt(slot.getAttribute('data-slot-id'), 10);
            
            slot.addEventListener('dragover', (e) => {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'move';
                slot.classList.add('drag-over');
            });
            
            slot.addEventListener('dragleave', () => {
                slot.classList.remove('drag-over');
            });
            
            slot.addEventListener('drop', (e) => {
                e.preventDefault();
                slot.classList.remove('drag-over');
                
                try {
                    const dataStr = e.dataTransfer.getData('text/plain');
                    if (!dataStr) return;
                    const data = JSON.parse(dataStr);
                    
                    if (data.teamPrefix !== teamPrefix) {
                        showToast('Cannot mix players between Home and Away teams!', 'error');
                        return;
                    }

                    const existingIndex = pitchSquads[teamPrefix].findIndex(p => p && p.name === data.name);
                    const currentOccupant = pitchSquads[teamPrefix][slotId];
                    
                    if (existingIndex !== -1) {
                        pitchSquads[teamPrefix][existingIndex] = currentOccupant;
                        pitchSquads[teamPrefix][slotId] = { name: data.name, position: data.position };
                    } else {
                        if (currentOccupant) {
                            setPlayerCardPlacedState(teamPrefix, currentOccupant.name, false);
                        }
                        pitchSquads[teamPrefix][slotId] = { name: data.name, position: data.position };
                        setPlayerCardPlacedState(teamPrefix, data.name, true);
                    }
                    
                    updatePitchSlotsDisplay(teamPrefix);
                } catch(err) {
                    console.error('Drop error:', err);
                }
            });
            
            slot.addEventListener('click', () => {
                const occupant = pitchSquads[teamPrefix][slotId];
                if (occupant) {
                    pitchSquads[teamPrefix][slotId] = null;
                    setPlayerCardPlacedState(teamPrefix, occupant.name, false);
                    updatePitchSlotsDisplay(teamPrefix);
                }
            });
        });
    }

    function setPlayerCardPlacedState(teamPrefix, playerName, isPlaced) {
        if (!playerName) return;
        const safeName = playerName.replace(/\s+/g, '-');
        const cardObj = document.getElementById(`dragcard-${teamPrefix}-${safeName}`);
        if (cardObj) {
            if (isPlaced) {
                cardObj.classList.add('placed');
            } else {
                cardObj.classList.remove('placed');
            }
        }
    }

    function updatePitchSlotsDisplay(teamPrefix) {
        const pitch = document.getElementById('pitch-' + teamPrefix);
        const countSpan = document.getElementById(teamPrefix + 'OccupiedCount');
        if (!pitch) return;
        
        const config = formationConfigs[currentFormation];
        let occupiedCount = 0;
        
        const slots = pitch.querySelectorAll('.pitch-slot');
        slots.forEach(slot => {
            const slotId = parseInt(slot.getAttribute('data-slot-id'), 10);
            const occupant = pitchSquads[teamPrefix][slotId];
            
            slot.innerHTML = '';
            
            if (occupant) {
                occupiedCount++;
                slot.classList.add('occupied');
                slot.title = "Click to remove";
                
                const labelSpan = document.createElement('span');
                labelSpan.className = 'slot-label';
                labelSpan.textContent = occupant.position || 'N/A';
                
                const nameSpan = document.createElement('span');
                nameSpan.className = 'player-name-label';
                nameSpan.textContent = occupant.name;
                
                slot.appendChild(labelSpan);
                slot.appendChild(nameSpan);
            } else {
                slot.classList.remove('occupied');
                slot.title = "Drag player here";
                
                const labelSpan = document.createElement('span');
                labelSpan.className = 'slot-label';
                labelSpan.textContent = config ? config.labels[slotId] : '';
                slot.appendChild(labelSpan);
            }
        });
        
        if (countSpan) countSpan.textContent = occupiedCount;
    }

    document.getElementById('homeTeamSelect').addEventListener('change', (e) => {
        renderAvailablePlayersForPitch(e.target.value, 'home');
    });

    document.getElementById('awayTeamSelect').addEventListener('change', (e) => {
        renderAvailablePlayersForPitch(e.target.value, 'away');
    });

    // Initialize slots on load
    setupPitchSlots();
    window.changeFormation('4-3-3');

    document.getElementById('runSimulationBtn').addEventListener('click', async () => {
        const homeTeam = document.getElementById('homeTeamSelect').value;
        const awayTeam = document.getElementById('awayTeamSelect').value;
        
        if (!homeTeam || !awayTeam) {
            showToast('Please select both Home and Away teams.', 'error');
            return;
        }

        const homeChecked = pitchSquads.home.filter(p => p !== null).map(p => p.name);
        const awayChecked = pitchSquads.away.filter(p => p !== null).map(p => p.name);

        if (homeChecked.length !== 11 || awayChecked.length !== 11) {
            showToast('You must fully build your squad (11 players) for both Home and Away teams.', 'error');
            return;
        }

        const requestBody = {
            homeTeam: homeTeam,
            awayTeam: awayTeam,
            homeSquad: homeChecked,
            awaySquad: awayChecked
        };

        const runBtn = document.getElementById('runSimulationBtn');
        const btnText = runBtn.querySelector('.btn-text');
        const btnSpinner = runBtn.querySelector('.btn-spinner');

        runBtn.disabled = true;
        btnSpinner.classList.remove('hidden');
        btnText.textContent = 'Simulating...';

        try {
            const response = await fetch('/api/simulation/match', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) throw new Error('Simulation failed');

            const result = await response.json();
            
            document.getElementById('simulationResults').classList.remove('hidden');
            
            document.getElementById('resScoreline').textContent = result.mostLikelyScoreline;
            document.getElementById('resCorrectScore').textContent = `${result.correctScorePct}%`;

            document.getElementById('resHomeWin').textContent = `${result.winHomePct}%`;
            document.getElementById('barHomeWin').style.width = `${result.winHomePct}%`;
            
            document.getElementById('resDraw').textContent = `${result.drawPct}%`;
            document.getElementById('barDraw').style.width = `${result.drawPct}%`;
            
            document.getElementById('resAwayWin').textContent = `${result.winAwayPct}%`;
            document.getElementById('barAwayWin').style.width = `${result.winAwayPct}%`;

            document.getElementById('resOver15').textContent = `${result.over15Pct}%`;
            document.getElementById('barOver15').style.width = `${result.over15Pct}%`;

            document.getElementById('resOver25').textContent = `${result.over25Pct}%`;
            document.getElementById('barOver25').style.width = `${result.over25Pct}%`;
            
            document.getElementById('resOver35').textContent = `${result.over35Pct}%`;
            document.getElementById('barOver35').style.width = `${result.over35Pct}%`;

            document.getElementById('resBttsYes').textContent = `${result.bttsYesPct}%`;
            document.getElementById('resCleanHome').textContent = `${result.cleanSheetHomePct}%`;
            document.getElementById('resCleanAway').textContent = `${result.cleanSheetAwayPct}%`;
            document.getElementById('resDcHome').textContent = `${result.doubleChanceHome}%`;
            document.getElementById('resDcAway').textContent = `${result.doubleChanceAway}%`;

            document.getElementById('resCornerOver85').textContent = `${result.cornerOver85Pct}%`;
            document.getElementById('barCornerOver85').style.width = `${result.cornerOver85Pct}%`;

            document.getElementById('resHomeXg').textContent = result.homeExpGoals.toFixed(2);
            document.getElementById('resAwayXg').textContent = result.awayExpGoals.toFixed(2);

            showToast('Simulation completed successfully!', 'success');
        } catch (error) {
            console.error('Error running simulation:', error);
            showToast('An error occurred during simulation.', 'error');
        } finally {
            runBtn.disabled = false;
            btnSpinner.classList.add('hidden');
            btnText.textContent = 'Run Simulation';
        }
    });

    // Compare Players Logic
    let compareChart = null;

    document.getElementById('runCompareBtn').addEventListener('click', async () => {
        const p1Name = document.getElementById('comparePlayer1').value.trim();
        const p2Name = document.getElementById('comparePlayer2').value.trim();

        if(!p1Name || !p2Name) {
            showToast('Please select both players to compare.', 'error');
            return;
        }

        const runBtn = document.getElementById('runCompareBtn');
        const btnText = runBtn.querySelector('.btn-text');
        const btnSpinner = runBtn.querySelector('.btn-spinner');

        runBtn.disabled = true;
        btnSpinner.classList.remove('hidden');
        btnText.textContent = 'Comparing...';

        try {
            const res1 = await fetch(`/api/players/scout?Name=${encodeURIComponent(p1Name)}`);
            const p1Data = await res1.json();
            
            const res2 = await fetch(`/api/players/scout?Name=${encodeURIComponent(p2Name)}`);
            const p2Data = await res2.json();

            const player1 = p1Data.find(p => p.name.toLowerCase() === p1Name.toLowerCase()) || p1Data[0];
            const player2 = p2Data.find(p => p.name.toLowerCase() === p2Name.toLowerCase()) || p2Data[0];

            if(!player1) {
                showToast(`Player '${p1Name}' not found.`, 'error');
                return;
            }
            if(!player2) {
                showToast(`Player '${p2Name}' not found.`, 'error');
                return;
            }

            document.getElementById('compareResults').classList.remove('hidden');
            renderCompareTable(player1, player2);
            renderCompareChart(player1, player2);
            
            showToast('Comparison successful!', 'success');

        } catch (error) {
            console.error(error);
            showToast('An error occurred while fetching player data.', 'error');
        } finally {
            runBtn.disabled = false;
            btnSpinner.classList.add('hidden');
            btnText.textContent = 'Compare';
        }
    });

    function renderCompareTable(p1, p2) {
        document.getElementById('th-player1').textContent = p1.name;
        document.getElementById('th-player2').textContent = p2.name;

        const stats = [
            { key: 'appearances', label: 'Appearances' },
            { key: 'minutesPlayed', label: 'Minutes Played' },
            { key: 'goals', label: 'Goals' },
            { key: 'assists', label: 'Assists' },
            { key: 'xg', label: 'Expected Goals (xG)' },
            { key: 'keyPasses', label: 'Key Passes' },
            { key: 'accuratePasses', label: 'Accurate Passes' },
            { key: 'tackles', label: 'Tackles' },
            { key: 'interceptions', label: 'Interceptions' },
            { key: 'clearances', label: 'Clearances' },
            { key: 'aerialsWon', label: 'Aerials Won' },
            { key: 'duelsWon', label: 'Duels Won' },
            { key: 'saves', label: 'Saves' }
        ];

        let html = '';
        stats.forEach(stat => {
            const val1 = p1[stat.key] || 0;
            const val2 = p2[stat.key] || 0;
            
            let class1 = 'equal-stat';
            let class2 = 'equal-stat';

            if(val1 > val2) {
                class1 = 'better-stat';
                class2 = 'worse-stat';
            } else if (val2 > val1) {
                class1 = 'worse-stat';
                class2 = 'better-stat';
            }

            html += `
                <tr>
                    <td class="${class1}">${val1}</td>
                    <td class="stat-name">${stat.label}</td>
                    <td class="${class2}">${val2}</td>
                </tr>
            `;
        });

        document.getElementById('compare-tbody').innerHTML = html;
    }

    function renderCompareChart(p1, p2) {
        const ctx = document.getElementById('compareRadarChart').getContext('2d');
        
        if(compareChart) {
            compareChart.destroy();
        }

        compareChart = new Chart(ctx, {
            type: 'radar',
            data: {
                labels: ['Goals', 'Assists', 'Key Passes', 'Tackles', 'Aerials Won', 'xG'],
                datasets: [
                    {
                        label: p1.name,
                        data: [
                            p1.goals || 0, 
                            p1.assists || 0, 
                            p1.keyPasses || 0, 
                            p1.tackles || 0, 
                            p1.aerialsWon || 0, 
                            p1.xg || 0
                        ],
                        backgroundColor: 'rgba(59, 130, 246, 0.2)',
                        borderColor: '#3b82f6',
                        pointBackgroundColor: '#3b82f6',
                        borderWidth: 2
                    },
                    {
                        label: p2.name,
                        data: [
                            p2.goals || 0, 
                            p2.assists || 0, 
                            p2.keyPasses || 0, 
                            p2.tackles || 0, 
                            p2.aerialsWon || 0, 
                            p2.xg || 0
                        ],
                        backgroundColor: 'rgba(16, 185, 129, 0.2)',
                        borderColor: '#10b981',
                        pointBackgroundColor: '#10b981',
                        borderWidth: 2
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        angleLines: { color: 'rgba(255, 255, 255, 0.1)' },
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        pointLabels: {
                            color: '#9ca3af',
                            font: { size: 12, family: 'Inter' }
                        },
                        ticks: {
                            display: false,
                            backdropColor: 'transparent'
                        }
                    }
                },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: '#f3f4f6',
                            font: { family: 'Inter' },
                            padding: 20
                        }
                    }
                }
            }
        });
    }

    // ------------------------------------------------------------------------
    // Team Management Logic (CRUD)
    // ------------------------------------------------------------------------
    let rosterPlayers = [];
    let deleteIdToConfirm = null;
    let editMode = false;

    const manageTeamSelect = document.getElementById('manageTeamSelect');
    const manageForm = document.getElementById('managePlayerForm');
    const formTitle = document.getElementById('formTitle');
    
    // Load roster on team change
    manageTeamSelect.addEventListener('change', async (e) => {
        const teamName = e.target.value;
        const tbody = document.getElementById('roster-tbody');
        const countSpan = document.getElementById('manageTeamCount');
        
        // Reset form to Add mode and preset the team name
        resetManageForm();
        document.getElementById('formTeamName').value = teamName || '';

        if (!teamName) {
            tbody.innerHTML = '<tr><td colspan="7" style="text-align: center; color: var(--text-muted);">Select a team to view roster.</td></tr>';
            countSpan.textContent = '0';
            return;
        }

        tbody.innerHTML = '<tr><td colspan="7" style="text-align: center;"><span class="btn-spinner">↻</span> Loading...</td></tr>';
        
        try {
            const res = await fetch(`/api/players/by-team?teamName=${encodeURIComponent(teamName)}`);
            if(!res.ok) throw new Error('Failed to fetch team roster');
            rosterPlayers = await res.json();
            
            countSpan.textContent = rosterPlayers.length;
            renderRosterTable();
        } catch(error) {
            console.error(error);
            showToast('Failed to load team roster.', 'error');
            tbody.innerHTML = '<tr><td colspan="7" style="text-align: center; color: #ef4444;">Error loading data.</td></tr>';
        }
    });

    function renderRosterTable() {
        const tbody = document.getElementById('roster-tbody');
        if (rosterPlayers.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" style="text-align: center; color: var(--text-muted);">No players found in this team.</td></tr>';
            return;
        }

        let html = '';
        rosterPlayers.forEach(p => {
            html += `
                <tr>
                    <td style="font-weight: 500;">${p.name}</td>
                    <td><span class="position-badge">${p.position}</span></td>
                    <td>${p.appearances}</td>
                    <td>${p.goals}</td>
                    <td>${p.assists}</td>
                    <td>${p.xg.toFixed(2)}</td>
                    <td style="text-align: right;">
                        <button class="action-btn" onclick="window.editPlayer(${p.statId})" title="Edit">✏️</button>
                        <button class="action-btn" onclick="window.confirmDelete(${p.statId})" title="Remove">🗑️</button>
                    </td>
                </tr>
            `;
        });
        tbody.innerHTML = html;
    }

    // Modal Handling for Deletion
    const confirmModal = document.getElementById('confirmModal');
    
    window.confirmDelete = function(id) {
        deleteIdToConfirm = id;
        confirmModal.classList.remove('hidden');
    };

    document.getElementById('modalCancelBtn').addEventListener('click', () => {
        deleteIdToConfirm = null;
        confirmModal.classList.add('hidden');
    });

    document.getElementById('modalConfirmBtn').addEventListener('click', async () => {
        if (!deleteIdToConfirm) return;
        
        const btn = document.getElementById('modalConfirmBtn');
        const oldText = btn.textContent;
        btn.textContent = 'Deleting...';
        btn.disabled = true;

        try {
            const res = await fetch(`/api/players/${deleteIdToConfirm}`, {
                method: 'DELETE'
            });

            if(!res.ok) throw new Error('Delete failed');
            
            showToast('Player removed successfully!', 'success');
            confirmModal.classList.add('hidden');
            
            // Reload the table
            manageTeamSelect.dispatchEvent(new Event('change'));
        } catch(err) {
            console.error(err);
            showToast('Failed to remove player.', 'error');
        } finally {
            btn.textContent = oldText;
            btn.disabled = false;
            deleteIdToConfirm = null;
        }
    });

    // Editing logic
    window.editPlayer = function(id) {
        const player = rosterPlayers.find(p => p.statId === id);
        if(!player) return;

        editMode = true;
        formTitle.textContent = 'Oyuncuyu Düzenle';
        
        document.getElementById('formStatId').value = player.statId;
        document.getElementById('formPlayerName').value = player.name;
        document.getElementById('formPosition').value = player.position;
        document.getElementById('formTeamName').value = manageTeamSelect.value;
        document.getElementById('formAppearances').value = player.appearances;
        document.getElementById('formGoals').value = player.goals;
        document.getElementById('formAssists').value = player.assists;
        document.getElementById('formXg').value = player.xg;
        document.getElementById('formKeyPasses').value = player.keyPasses;
        document.getElementById('formTackles').value = player.tackles;
        document.getElementById('formInterceptions').value = player.interceptions;
        document.getElementById('formAerialsWon').value = player.aerialsWon;
        document.getElementById('formMinutesPlayed').value = player.minutesPlayed;
        
        document.querySelector('.player-form-container').scrollIntoView({ behavior: 'smooth' });
    };

    function resetManageForm() {
        editMode = false;
        formTitle.textContent = 'Yeni Oyuncu Ekle';
        manageForm.reset();
        document.getElementById('formStatId').value = '';
    }

    document.getElementById('manageCancelBtn').addEventListener('click', () => {
        resetManageForm();
        document.getElementById('formTeamName').value = manageTeamSelect.value || '';
    });

    // Form Submission (Create or Update)
    manageForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const submitBtn = document.getElementById('manageSaveBtn');
        const btnText = submitBtn.querySelector('.btn-text');
        const btnSpinner = submitBtn.querySelector('.btn-spinner');
        
        submitBtn.disabled = true;
        btnSpinner.classList.remove('hidden');
        btnText.textContent = 'Saving...';

        const statId = document.getElementById('formStatId').value;
        const payload = {
            StatId: statId ? parseInt(statId) : 0,
            PlayerName: document.getElementById('formPlayerName').value.trim(),
            TeamName: document.getElementById('formTeamName').value.trim(),
            Position: document.getElementById('formPosition').value,
            Appearances: parseInt(document.getElementById('formAppearances').value) || 0,
            Goals: parseInt(document.getElementById('formGoals').value) || 0,
            Assists: parseInt(document.getElementById('formAssists').value) || 0,
            Xg: parseFloat(document.getElementById('formXg').value) || 0,
            KeyPasses: parseInt(document.getElementById('formKeyPasses').value) || 0,
            Tackles: parseInt(document.getElementById('formTackles').value) || 0,
            Interceptions: parseInt(document.getElementById('formInterceptions').value) || 0,
            AerialsWon: parseInt(document.getElementById('formAerialsWon').value) || 0,
            MinutesPlayed: parseInt(document.getElementById('formMinutesPlayed').value) || 0
        };

        const url = editMode ? `/api/players/${statId}` : '/api/players';
        const method = editMode ? 'PUT' : 'POST';

        try {
            const res = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if(!res.ok) throw new Error('Save failed');

            showToast(editMode ? 'Player updated successfully!' : 'Player added successfully!', 'success');
            
            // Reload the table (if the team is still the same, or reload entirely)
            manageTeamSelect.value = payload.TeamName;
            manageTeamSelect.dispatchEvent(new Event('change'));
            
        } catch (err) {
            console.error(err);
            showToast('Failed to save player.', 'error');
        } finally {
            submitBtn.disabled = false;
            btnSpinner.classList.add('hidden');
            btnText.textContent = 'Save Player';
        }
    });

    // Initial load
    loadRatings();
    loadTeamStats();
    loadAllBasicPlayers();
});
