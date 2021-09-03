Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports MinMaxStrategy = org.nd4j.linalg.dataset.api.preprocessor.MinMaxStrategy
Imports MultiNormalizerHybrid = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerHybrid
Imports org.nd4j.linalg.dataset.api.preprocessor
Imports StandardizeStrategy = org.nd4j.linalg.dataset.api.preprocessor.StandardizeStrategy
Imports DistributionStats = org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats
Imports MinMaxStats = org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.serializer


	Public Class MultiHybridSerializerStrategy
		Implements NormalizerSerializerStrategy(Of MultiNormalizerHybrid)

		''' <summary>
		''' Serialize a MultiNormalizerHybrid to a output stream
		''' </summary>
		''' <param name="normalizer"> the normalizer </param>
		''' <param name="stream">     the output stream to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void write(@NonNull MultiNormalizerHybrid normalizer, @NonNull OutputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub write(ByVal normalizer As MultiNormalizerHybrid, ByVal stream As Stream)
			Using dos As New DataOutputStream(stream)
				writeStatsMap(normalizer.getInputStats(), dos)
				writeStatsMap(normalizer.getOutputStats(), dos)
				writeStrategy(normalizer.getGlobalInputStrategy(), dos)
				writeStrategy(normalizer.getGlobalOutputStrategy(), dos)
				writeStrategyMap(normalizer.getPerInputStrategies(), dos)
				writeStrategyMap(normalizer.getPerOutputStrategies(), dos)
			End Using
		End Sub

		''' <summary>
		''' Restore a MultiNormalizerHybrid that was previously serialized by this strategy
		''' </summary>
		''' <param name="stream"> the input stream to restore from </param>
		''' <returns> the restored MultiNormalizerStandardize </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerHybrid restore(@NonNull InputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function restore(ByVal stream As Stream) As MultiNormalizerHybrid
			Dim dis As New DataInputStream(stream)

			Dim result As New MultiNormalizerHybrid()
			result.setInputStats(readStatsMap(dis))
			result.setOutputStats(readStatsMap(dis))
			result.setGlobalInputStrategy(readStrategy(dis))
			result.setGlobalOutputStrategy(readStrategy(dis))
			result.setPerInputStrategies(readStrategyMap(dis))
			result.setPerOutputStrategies(readStrategyMap(dis))

			Return result
		End Function

		Public Overridable ReadOnly Property SupportedType As NormalizerType Implements NormalizerSerializerStrategy(Of MultiNormalizerHybrid).getSupportedType
			Get
				Return NormalizerType.MULTI_HYBRID
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeStatsMap(java.util.Map<Integer, org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats> statsMap, DataOutputStream dos) throws IOException
		Private Shared Sub writeStatsMap(ByVal statsMap As IDictionary(Of Integer, NormalizerStats), ByVal dos As DataOutputStream)
			Dim indices As ISet(Of Integer) = statsMap.Keys
			dos.writeInt(indices.Count)
			For Each index As Integer In indices
				dos.writeInt(index)
				writeNormalizerStats(statsMap(index), dos)
			Next index
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.util.Map<Integer, org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats> readStatsMap(DataInputStream dis) throws IOException
		Private Shared Function readStatsMap(ByVal dis As DataInputStream) As IDictionary(Of Integer, NormalizerStats)
			Dim result As IDictionary(Of Integer, NormalizerStats) = New Dictionary(Of Integer, NormalizerStats)()
			Dim numEntries As Integer = dis.readInt()
			For i As Integer = 0 To numEntries - 1
				Dim index As Integer = dis.readInt()
				result(index) = readNormalizerStats(dis)
			Next i
			Return result
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeNormalizerStats(org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats normalizerStats, DataOutputStream dos) throws IOException
		Private Shared Sub writeNormalizerStats(ByVal normalizerStats As NormalizerStats, ByVal dos As DataOutputStream)
			If TypeOf normalizerStats Is DistributionStats Then
				writeDistributionStats(DirectCast(normalizerStats, DistributionStats), dos)
			ElseIf TypeOf normalizerStats Is MinMaxStats Then
				writeMinMaxStats(DirectCast(normalizerStats, MinMaxStats), dos)
			Else
				Throw New Exception("Unsupported stats class " & normalizerStats.GetType())
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats readNormalizerStats(DataInputStream dis) throws IOException
		Private Shared Function readNormalizerStats(ByVal dis As DataInputStream) As NormalizerStats
			Dim strategy As Strategy = System.Enum.GetValues(GetType(Strategy))(dis.readInt())
			Select Case strategy
				Case org.nd4j.linalg.dataset.api.preprocessor.serializer.MultiHybridSerializerStrategy.Strategy.STANDARDIZE
					Return readDistributionStats(dis)
				Case org.nd4j.linalg.dataset.api.preprocessor.serializer.MultiHybridSerializerStrategy.Strategy.MIN_MAX
					Return readMinMaxStats(dis)
				Case Else
					Throw New Exception("Unsupported strategy " & strategy.ToString())
			End Select
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeDistributionStats(org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats normalizerStats, DataOutputStream dos) throws IOException
		Private Shared Sub writeDistributionStats(ByVal normalizerStats As DistributionStats, ByVal dos As DataOutputStream)
			dos.writeInt(Strategy.STANDARDIZE.ordinal())
			Nd4j.write(normalizerStats.getMean(), dos)
			Nd4j.write(normalizerStats.getStd(), dos)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats readDistributionStats(DataInputStream dis) throws IOException
		Private Shared Function readDistributionStats(ByVal dis As DataInputStream) As NormalizerStats
			Return New DistributionStats(Nd4j.read(dis), Nd4j.read(dis))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeMinMaxStats(org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats normalizerStats, DataOutputStream dos) throws IOException
		Private Shared Sub writeMinMaxStats(ByVal normalizerStats As MinMaxStats, ByVal dos As DataOutputStream)
			dos.writeInt(Strategy.MIN_MAX.ordinal())
			Nd4j.write(normalizerStats.getLower(), dos)
			Nd4j.write(normalizerStats.getUpper(), dos)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats readMinMaxStats(DataInputStream dis) throws IOException
		Private Shared Function readMinMaxStats(ByVal dis As DataInputStream) As NormalizerStats
			Return New MinMaxStats(Nd4j.read(dis), Nd4j.read(dis))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeStrategyMap(java.util.Map<Integer, org.nd4j.linalg.dataset.api.preprocessor.NormalizerStrategy> strategyMap, DataOutputStream dos) throws IOException
		Private Shared Sub writeStrategyMap(ByVal strategyMap As IDictionary(Of Integer, NormalizerStrategy), ByVal dos As DataOutputStream)
			Dim indices As ISet(Of Integer) = strategyMap.Keys
			dos.writeInt(indices.Count)

			For Each index As Integer In indices
				dos.writeInt(index)
				writeStrategy(strategyMap(index), dos)
			Next index
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.util.Map<Integer, org.nd4j.linalg.dataset.api.preprocessor.NormalizerStrategy> readStrategyMap(DataInputStream dis) throws IOException
		Private Shared Function readStrategyMap(ByVal dis As DataInputStream) As IDictionary(Of Integer, NormalizerStrategy)
			Dim result As IDictionary(Of Integer, NormalizerStrategy) = New Dictionary(Of Integer, NormalizerStrategy)()
			Dim numIndices As Integer = dis.readInt()
			For i As Integer = 0 To numIndices - 1
				result(dis.readInt()) = readStrategy(dis)
			Next i
			Return result
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeStrategy(org.nd4j.linalg.dataset.api.preprocessor.NormalizerStrategy strategy, DataOutputStream dos) throws IOException
		Private Shared Sub writeStrategy(ByVal strategy As NormalizerStrategy, ByVal dos As DataOutputStream)
			If strategy Is Nothing Then
				writeNoStrategy(dos)
			ElseIf TypeOf strategy Is StandardizeStrategy Then
				writeStandardizeStrategy(dos)
			ElseIf TypeOf strategy Is MinMaxStrategy Then
				writeMinMaxStrategy(CType(strategy, MinMaxStrategy), dos)
			Else
				Throw New Exception("Unsupported strategy class " & strategy.GetType())
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.dataset.api.preprocessor.NormalizerStrategy readStrategy(DataInputStream dis) throws IOException
		Private Shared Function readStrategy(ByVal dis As DataInputStream) As NormalizerStrategy
			Dim strategy As Strategy = System.Enum.GetValues(GetType(Strategy))(dis.readInt())
			Select Case strategy
				Case org.nd4j.linalg.dataset.api.preprocessor.serializer.MultiHybridSerializerStrategy.Strategy.NULL
					Return Nothing
				Case org.nd4j.linalg.dataset.api.preprocessor.serializer.MultiHybridSerializerStrategy.Strategy.STANDARDIZE
					Return readStandardizeStrategy()
				Case org.nd4j.linalg.dataset.api.preprocessor.serializer.MultiHybridSerializerStrategy.Strategy.MIN_MAX
					Return readMinMaxStrategy(dis)
				Case Else
					Throw New Exception("Unsupported strategy " & strategy.ToString())
			End Select
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeNoStrategy(DataOutputStream dos) throws IOException
		Private Shared Sub writeNoStrategy(ByVal dos As DataOutputStream)
			dos.writeInt(Strategy.NULL.ordinal())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeStandardizeStrategy(DataOutputStream dos) throws IOException
		Private Shared Sub writeStandardizeStrategy(ByVal dos As DataOutputStream)
			dos.writeInt(Strategy.STANDARDIZE.ordinal())
		End Sub

		Private Shared Function readStandardizeStrategy() As NormalizerStrategy
			Return New StandardizeStrategy()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeMinMaxStrategy(org.nd4j.linalg.dataset.api.preprocessor.MinMaxStrategy strategy, DataOutputStream dos) throws IOException
		Private Shared Sub writeMinMaxStrategy(ByVal strategy As MinMaxStrategy, ByVal dos As DataOutputStream)
			dos.writeInt(Strategy.MIN_MAX.ordinal())
			dos.writeDouble(strategy.getMinRange())
			dos.writeDouble(strategy.getMaxRange())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.dataset.api.preprocessor.NormalizerStrategy readMinMaxStrategy(DataInputStream dis) throws IOException
		Private Shared Function readMinMaxStrategy(ByVal dis As DataInputStream) As NormalizerStrategy
			Return New MinMaxStrategy(dis.readDouble(), dis.readDouble())
		End Function

		''' <summary>
		''' This enum is exclusively used for ser/de purposes in this serializer, for indicating the opType of normalizer
		''' strategy used for an input/output or global settings.
		''' 
		''' NOTE: ONLY EVER CONCATENATE NEW VALUES AT THE BOTTOM!
		''' 
		''' The data format depends on the ordinal values of the enum values. Therefore, removing a value or adding one
		''' in between existing values will corrupt normalizers serialized with previous versions.
		''' </summary>
		Private Enum Strategy
			NULL
			STANDARDIZE
			MIN_MAX
		End Enum
	End Class

End Namespace