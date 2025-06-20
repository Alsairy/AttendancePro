{{/*
Expand the name of the chart.
*/}}
{{- define "attendancepro.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
*/}}
{{- define "attendancepro.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "attendancepro.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "attendancepro.labels" -}}
helm.sh/chart: {{ include "attendancepro.chart" . }}
{{ include "attendancepro.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "attendancepro.selectorLabels" -}}
app.kubernetes.io/name: {{ include "attendancepro.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "attendancepro.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "attendancepro.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Database connection string
*/}}
{{- define "attendancepro.databaseConnectionString" -}}
{{- if .Values.postgresql.enabled }}
{{- printf "Server=%s-postgresql;Database=%s;User Id=postgres;Password=%s;TrustServerCertificate=true" .Release.Name .Values.postgresql.auth.database .Values.postgresql.auth.postgresPassword }}
{{- else }}
{{- .Values.externalDatabase.connectionString }}
{{- end }}
{{- end }}

{{/*
Redis connection string
*/}}
{{- define "attendancepro.redisConnectionString" -}}
{{- if .Values.redis.enabled }}
{{- printf "%s-redis-master:6379" .Release.Name }}
{{- else }}
{{- .Values.externalRedis.connectionString }}
{{- end }}
{{- end }}

{{/*
Image name helper
*/}}
{{- define "attendancepro.image" -}}
{{- $registry := .Values.global.imageRegistry | default .Values.image.registry -}}
{{- $repository := .repository -}}
{{- $tag := .tag | default .Values.image.tag | default .Chart.AppVersion -}}
{{- printf "%s/%s:%s" $registry $repository $tag -}}
{{- end }}

