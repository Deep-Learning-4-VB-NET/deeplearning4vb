Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.versioncheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class VersionCheck
	Public Class VersionCheck
		<Obsolete("Use <seealso cref=""ND4JSystemProperties.VERSION_CHECK_PROPERTY""/>")>
		Public Const VERSION_CHECK_PROPERTY As String = ND4JSystemProperties.VERSION_CHECK_PROPERTY
		Public Const GIT_PROPERTY_FILE_SUFFIX As String = "-git.properties"

		Private Const SCALA_210_SUFFIX As String = "_2.10"
		Private Const SCALA_211_SUFFIX As String = "_2.11"
		Private Const SPARK_1_VER_STRING As String = "spark_1"
		Private Const SPARK_2_VER_STRING As String = "spark_2"

		Private Const UNKNOWN_VERSION As String = "(Unknown)"

		Private Const DL4J_GROUPID As String = "org.deeplearning4j"
		Private Const DL4J_ARTIFACT As String = "deeplearning4j-nn"

		Private Const DATAVEC_GROUPID As String = "org.datavec"
		Private Const DATAVEC_ARTIFACT As String = "datavec-api"

		Private Const ND4J_GROUPID As String = "org.nd4j"

		Private Const ND4J_JBLAS_CLASS As String = "org.nd4j.linalg.jblas.JblasBackend"
		Private Const CANOVA_CLASS As String = "org.canova.api.io.data.DoubleWritable"

		Private Shared ReadOnly GROUPIDS_TO_CHECK As ISet(Of String) = New HashSet(Of String)(Arrays.asList(ND4J_GROUPID, DL4J_GROUPID, DATAVEC_GROUPID)) 'NOTE: DL4J_GROUPID also covers Arbiter and RL4J

		''' <summary>
		''' Detailed level for logging:
		''' GAV: display group ID, artifact, version
		''' GAVC: display group ID, artifact, version, commit ID
		''' FULL: display group ID, artifact, version, commit ID, build time, branch, commit message
		''' </summary>
		Public Enum Detail
			GAV
			GAVC
			FULL
		End Enum

		Private Sub New()

		End Sub

		''' <summary>
		''' Perform a check of the versions of ND4J, DL4J, DataVec, RL4J and Arbiter dependencies, logging a warning
		''' if necessary.
		''' </summary>
		Public Shared Sub checkVersions()
			Dim doCheck As Boolean = Boolean.Parse(System.getProperty(ND4JSystemProperties.VERSION_CHECK_PROPERTY, "true"))

			If Not doCheck Then
				Return
			End If

			If ND4JClassLoading.classPresentOnClasspath(ND4J_JBLAS_CLASS) Then
				'nd4j-jblas is ancient and incompatible
				log.error("Found incompatible/obsolete backend and version (nd4j-jblas) on classpath. ND4J is unlikely to" & " function correctly with nd4j-jblas on the classpath. JVM will now exit.")
				Environment.Exit(1)
			End If

			If ND4JClassLoading.classPresentOnClasspath(CANOVA_CLASS) Then
				'Canova is ancient and likely to pull in incompatible dependencies
				log.error("Found incompatible/obsolete library Canova on classpath. ND4J is unlikely to" & " function correctly with this library on the classpath. JVM will now exit.")
				Environment.Exit(1)
			End If

			Dim dependencies As IList(Of VersionInfo) = getVersionInfos()
			If dependencies.Count <= 2 Then
				'No -properties.git files were found on the classpath. This may be due to a misconfigured uber-jar
				' or maybe running in IntelliJ with "dynamic.classpath" set to true (in workspace.xml). Either way,
				' we can't check versions and don't want to log an error, which will more often than not be wrong
				If dependencies.Count = 0 Then
					Return
				End If

				'Another edge case: no -properties.git files were found, but DL4J and/or DataVec were inferred
				' by class names. If these "inferred by opName" versions were the only things found, we should also
				' not log a warning, as we can't check versions in this case

				Dim dl4jViaClass As Boolean = False
				Dim datavecViaClass As Boolean = False
				For Each vi As VersionInfo In dependencies
					If DL4J_GROUPID.Equals(vi.getGroupId()) AndAlso DL4J_ARTIFACT.Equals(vi.getArtifactId()) AndAlso (UNKNOWN_VERSION.Equals(vi.getBuildVersion())) Then
						dl4jViaClass = True
					ElseIf DATAVEC_GROUPID.Equals(vi.getGroupId()) AndAlso DATAVEC_ARTIFACT.Equals(vi.getArtifactId()) AndAlso (UNKNOWN_VERSION.Equals(vi.getBuildVersion())) Then
						datavecViaClass = True
					End If
				Next vi

				If dependencies.Count = 1 AndAlso (dl4jViaClass OrElse datavecViaClass) Then
					Return
				ElseIf dependencies.Count = 2 AndAlso dl4jViaClass AndAlso datavecViaClass Then
					Return
				End If
			End If

			Dim foundVersions As ISet(Of String) = New HashSet(Of String)()
			For Each vi As VersionInfo In dependencies
				Dim g As String = vi.getGroupId()
				If g IsNot Nothing AndAlso GROUPIDS_TO_CHECK.Contains(g) Then
					Dim version As String = vi.getBuildVersion()

					If version.Contains("_spark_") Then
						'Normalize spark versions:
						' "0.9.1_spark_1" to "0.9.1" and "0.9.1_spark_1-SNAPSHOT" to "0.9.1-SNAPSHOT"
						version = version.replaceAll("_spark_1","")
						version = version.replaceAll("_spark_2","")
					End If

					foundVersions.Add(version)
				End If
			Next vi

			Dim logVersions As Boolean = False

			If foundVersions.Count > 1 Then
				log.warn("*** ND4J VERSION CHECK FAILED - INCOMPATIBLE VERSIONS FOUND ***")
				log.warn("Incompatible versions (different version number) of DL4J, ND4J, RL4J, DataVec, Arbiter are unlikely to function correctly")
				logVersions = True
			End If

			'Also: check for mixed scala versions - but only for our dependencies... These are in the artifact ID,
			' scored like dl4j-spack_2.10 and deeplearning4j-ui_2.11
			'And check for mixed spark versions (again, just DL4J/DataVec etc dependencies for now)
			Dim scala210 As Boolean = False
			Dim scala211 As Boolean = False
			Dim spark1 As Boolean = False
			Dim spark2 As Boolean = False
			For Each vi As VersionInfo In dependencies
				Dim artifact As String = vi.getArtifactId()
				If Not scala210 AndAlso artifact.Contains(SCALA_210_SUFFIX) Then
					scala210 = True
				End If
				If Not scala211 AndAlso artifact.Contains(SCALA_211_SUFFIX) Then
					scala211 = True
				End If

				Dim version As String = vi.getBuildVersion()
				If Not spark1 AndAlso version.Contains(SPARK_1_VER_STRING) Then
					spark1 = True
				End If
				If Not spark2 AndAlso version.Contains(SPARK_2_VER_STRING) Then
					spark2 = True
				End If
			Next vi

			If scala210 AndAlso scala211 Then
				log.warn("*** ND4J VERSION CHECK FAILED - FOUND BOTH SCALA VERSION 2.10 AND 2.11 ARTIFACTS ***")
				log.warn("Projects with mixed Scala versions (2.10/2.11) are unlikely to function correctly")
				logVersions = True
			End If

			If spark1 AndAlso spark2 Then
				log.warn("*** ND4J VERSION CHECK FAILED - FOUND BOTH SPARK VERSION 1 AND 2 ARTIFACTS ***")
				log.warn("Projects with mixed Spark versions (1 and 2) are unlikely to function correctly")
				logVersions = True
			End If

			If logVersions Then
				log.info("Versions of artifacts found on classpath:")
				logVersionInfo()
			End If
		End Sub

		''' <returns> A list of the property files containing the build/version info </returns>
		Public Shared Function listGitPropertiesFiles() As IList(Of URI)
			Dim roots As IEnumerator(Of URL)
			Try
				roots = GetType(VersionCheck).getClassLoader().getResources("ai/skymind/")
			Catch e As IOException
				'Should never happen?
				log.debug("Error listing resources for version check", e)
				Return Collections.emptyList()
			End Try

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<java.net.URI> out = new java.util.ArrayList<>();
			Dim [out] As IList(Of URI) = New List(Of URI)()
			Do While roots.MoveNext()
				Dim u As URL = roots.Current

				Try
					Dim uri As URI = u.toURI()
					Using fileSystem As java.nio.file.FileSystem = (If(uri.getScheme().Equals("jar"), java.nio.file.FileSystems.newFileSystem(uri, Enumerable.Empty(Of String, Object)()), Nothing))
						Dim myPath As Path = Paths.get(uri)
						Files.walkFileTree(myPath, New SimpleFileVisitorAnonymousInnerClass([out]))
					End Using
				Catch e As NoClassDefFoundError
					'Should only happen on Android 7.0 or earlier - silently ignore
					'https://github.com/eclipse/deeplearning4j/issues/6609
				Catch e As Exception
					'log and skip
					log.debug("Error finding/loading version check resources", e)
				End Try
			Loop

			[out].Sort() 'Equivalent to sorting by groupID and artifactID
			Return [out]
		End Function

		Private Class SimpleFileVisitorAnonymousInnerClass
			Inherits SimpleFileVisitor(Of Path)

			Private [out] As IList(Of URI)

			Public Sub New(ByVal [out] As IList(Of URI))
				Me.out = [out]
			End Sub

			Public Overrides Function visitFile(ByVal file As Path, ByVal attrs As BasicFileAttributes) As FileVisitResult
				Dim fileUri As URI = file.toUri()
				Dim s As String = fileUri.ToString()
				If s.EndsWith(GIT_PROPERTY_FILE_SUFFIX, StringComparison.Ordinal) Then
					[out].Add(fileUri)
				End If
				Return FileVisitResult.CONTINUE
			End Function
		End Class

		''' <returns> A list containing the information for the discovered dependencies </returns>
		Public Shared ReadOnly Property VersionInfos As IList(Of VersionInfo)
			Get
    
				Dim dl4jFound As Boolean = False
				Dim datavecFound As Boolean = False
    
				Dim repState As IList(Of VersionInfo) = New List(Of VersionInfo)()
				For Each s As URI In listGitPropertiesFiles()
					Dim grs As VersionInfo
    
					Try
						grs = New VersionInfo(s)
					Catch e As Exception
						log.debug("Error reading property files for {}", s)
						Continue For
					End Try
					repState.Add(grs)
    
					If Not dl4jFound AndAlso DL4J_GROUPID.Equals(grs.getGroupId(), StringComparison.OrdinalIgnoreCase) AndAlso DL4J_ARTIFACT.Equals(grs.getArtifactId(), StringComparison.OrdinalIgnoreCase) Then
						dl4jFound = True
					End If
    
					If Not datavecFound AndAlso DATAVEC_GROUPID.Equals(grs.getGroupId(), StringComparison.OrdinalIgnoreCase) AndAlso DATAVEC_ARTIFACT.Equals(grs.getArtifactId(), StringComparison.OrdinalIgnoreCase) Then
						datavecFound = True
					End If
				Next s
    
				If ND4JClassLoading.classPresentOnClasspath(ND4J_JBLAS_CLASS) Then
					'nd4j-jblas is ancient and incompatible
					log.error("Found incompatible/obsolete backend and version (nd4j-jblas) on classpath. ND4J is unlikely to" & " function correctly with nd4j-jblas on the classpath.")
				End If
    
				If ND4JClassLoading.classPresentOnClasspath(CANOVA_CLASS) Then
					'Canova is anchient and likely to pull in incompatible
					log.error("Found incompatible/obsolete library Canova on classpath. ND4J is unlikely to" & " function correctly with this library on the classpath.")
				End If
    
				Return repState
			End Get
		End Property

		''' <returns> A string representation of the version information, with the default (GAV) detail level </returns>
		Public Shared Function versionInfoString() As String
			Return versionInfoString(Detail.GAV)
		End Function

		''' <summary>
		''' Get the version information for dependencies as a string with a specified amount of detail
		''' </summary>
		''' <param name="detail"> Detail level for the version information. See <seealso cref="Detail"/> </param>
		''' <returns> Version information, as a String </returns>
		Public Shared Function versionInfoString(ByVal detail As Detail) As String
			Dim sb As New StringBuilder()
			For Each grp As VersionInfo In getVersionInfos()
				sb.Append(grp.getGroupId()).Append(" : ").Append(grp.getArtifactId()).Append(" : ").Append(grp.getBuildVersion())
				Select Case detail
					Case org.nd4j.versioncheck.VersionCheck.Detail.FULL, GAVC
						sb.Append(" - ").Append(grp.getCommitIdAbbrev())
						If detail <> Detail.FULL Then
							Exit Select
						End If

						sb.Append("buildTime=").Append(grp.getBuildTime()).Append("branch=").Append(grp.getBranch()).Append("commitMsg=").Append(grp.getCommitMessageShort())
				End Select
				sb.Append(vbLf)
			Next grp
			Return sb.ToString()
		End Function

		''' <summary>
		''' Log of the version information with the default level of detail
		''' </summary>
		Public Shared Sub logVersionInfo()
			logVersionInfo(Detail.GAV)
		End Sub

		''' <summary>
		''' Log the version information with the specified level of detail </summary>
		''' <param name="detail"> Level of detail for logging </param>
		Public Shared Sub logVersionInfo(ByVal detail As Detail)

			Dim info As IList(Of VersionInfo) = getVersionInfos()

			For Each grp As VersionInfo In info
				Select Case detail
					Case org.nd4j.versioncheck.VersionCheck.Detail.GAV
						log.info("{} : {} : {}", grp.getGroupId(), grp.getArtifactId(), grp.getBuildVersion())
					Case org.nd4j.versioncheck.VersionCheck.Detail.GAVC
						log.info("{} : {} : {} - {}", grp.getGroupId(), grp.getArtifactId(), grp.getBuildVersion(), grp.getCommitIdAbbrev())
					Case org.nd4j.versioncheck.VersionCheck.Detail.FULL
						log.info("{} : {} : {} - {}, buildTime={}, buildHost={} branch={}, commitMsg={}", grp.getGroupId(), grp.getArtifactId(), grp.getBuildVersion(), grp.getCommitId(), grp.getBuildTime(), grp.getBuildHost(), grp.getBranch(), grp.getCommitMessageShort())
				End Select
			Next grp
		End Sub
	End Class

End Namespace