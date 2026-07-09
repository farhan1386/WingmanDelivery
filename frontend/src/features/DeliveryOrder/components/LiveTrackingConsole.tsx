import { useEffect, useState } from "react";
import type { DeliveryLogsModel } from "../../DeliveryOrderLogs/types";
import type { GridDataModel, FilterModel } from "../../shared/types";

export function LiveTrackingConsole() {
    const [logs, setLogs] = useState<DeliveryLogsModel[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [isLive, setIsLive] = useState<boolean>(true);

    // Pagination and query filtering bounds matching WingmanDelivery.Models.FilterModel
    const [gridQuery] = useState<FilterModel>({
        FilterString: null,
        SearchValue: null,
        OrderField: "f_create_date",
        Order: "DESC",
        Skip: 0,
        Take: 10,
    });

    // Main telemetry polling mechanism
    useEffect(() => {
        fetchLogsStream();

        if (!isLive) return;

        // Poll the Dapper transactional logs table every 5 seconds
        const interval = setInterval(() => {
            fetchLogsStream();
        }, 5000);

        return () => clearInterval(interval);
    }, [isLive]);

    const fetchLogsStream = async () => {
        setIsLoading(true);
        setError(null);

        try {
            // Calls your DeliveryOrderLogsEndpoints module POST: /api/delivery-logs/grid
            const response = await fetch("/api/delivery-logs/grid", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(gridQuery),
            });

            if (!response.ok) {
                throw new Error(`Server returned execution exception: ${response.status}`);
            }

            // Explicit type casting guarantees schema compliance
            const data = (await response.json()) as GridDataModel<DeliveryLogsModel>;
            setLogs(data.Items || []);
        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : String(err);
            console.error("Telemetry streaming fault:", err);
            setError(`Telemetry connection down: ${errorMessage}`);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: "700px", margin: "30px auto", fontFamily: "Segoe UI, -apple-system, sans-serif" }}>
            <div style={{ padding: "25px", border: "1px solid #e2e8f0", borderRadius: "12px", background: "#ffffff", boxShadow: "0 4px 6px -1px rgba(0,0,0,0.05)" }}>

                {/* Header Action Section */}
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "15px" }}>
                    <div style={{ display: "flex", alignItems: "center", gap: "10px" }}>
                        <span style={{ fontSize: "24px" }}>📡</span>
                        <h3 style={{ color: "#0f172a", margin: 0, fontSize: "18px", fontWeight: 600 }}>Live Delivery Operations Console</h3>
                    </div>

                    <button
                        type="button"
                        onClick={() => setIsLive((prev) => !prev)}
                        style={{
                            padding: "6px 14px",
                            borderRadius: "20px",
                            border: "1px solid #cbd5e1",
                            fontSize: "13px",
                            fontWeight: 600,
                            cursor: "pointer",
                            backgroundColor: isLive ? "#ef4444" : "#22c55e",
                            color: "#ffffff",
                        }}
                    >
                        {isLive ? "⏸️ Pause Live Feed" : "▶️ Resume Stream"}
                    </button>
                </div>

                <p style={{ color: "#64748b", fontSize: "14px", marginTop: 0, marginBottom: "20px" }}>
                    Displaying active transactional telemetry streams intercepted directly out of the Dapper execution layer engine.
                </p>

                {error && (
                    <div style={{ color: "#b91c1c", padding: "10px 14px", backgroundColor: "#fef2f2", border: "1px solid #fee2e2", borderRadius: "6px", marginBottom: "15px", fontSize: "13px" }}>
                        ⚠️ {error}
                    </div>
                )}

                {/* Telemetry Stream Display Feed */}
                <div style={{ display: "flex", flexDirection: "column", gap: "12px", maxHeight: "350px", overflowY: "auto", paddingRight: "5px" }}>
                    {logs.length === 0 ? (
                        <p style={{ textAlign: "center", color: "#94a3b8", fontSize: "14px", padding: "20px 0" }}>
                            {isLoading ? "Scanning log buffers..." : "No operational entries tracked inside database records."}
                        </p>
                    ) : (
                        logs.map((log) => (
                            <div
                                key={log.f_uid}
                                style={{
                                    padding: "12px 16px",
                                    borderRadius: "8px",
                                    border: "1px solid #e2e8f0",
                                    backgroundColor: "#f8fafc",
                                    fontSize: "14px",
                                }}
                            >
                                <div style={{ display: "flex", justifyContent: "space-between", marginBottom: "6px" }}>
                                    <span style={{ color: "#0284c7", fontWeight: 600, fontFamily: "monospace" }}>
                                        Delivery Context ID: {log.f_delivery_uid.substring(0, 8)}...
                                    </span>
                                    <span style={{ color: "#94a3b8", fontSize: "12px" }}>
                                        {log.f_create_date ? new Date(log.f_create_date).toLocaleTimeString() : "Pending Time"}
                                    </span>
                                </div>
                                <div style={{ color: "#334155" }}>
                                    Status boundary mutation map shifting transition states from index{" "}
                                    <span style={{ fontWeight: 600, color: "#e11d48" }}>{log.f_status_from}</span> to{" "}
                                    <span style={{ fontWeight: 600, color: "#16a34a" }}>{log.f_status_to}</span>.
                                </div>
                            </div>
                        ))
                    )}
                </div>

            </div>
        </div>
    );
}