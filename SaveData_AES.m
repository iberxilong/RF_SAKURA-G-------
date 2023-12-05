clear;
clc;
%����AES������
name='����8����Ե�Ƽ� - SAKURA-G������ɲ����\DSO-X 3034T WaveData 2021_11_17_19h12m50s\';
filetime='DSO-X 3034T WaveData 2021_11_17_19h12m50s';
C=[];
key=[];
cipher=[];
plaintext=[];
traces=[];
% AA=[];
% CC=[];

fid = fopen(fullfile(['E:\',name,'Atext.csv']), 'r');  %����һ���ļ�λ�� 

tracesNumber=10016;
for numtraces=1:tracesNumber
    % % ��Կ��ȡ
    clear d;
    keylen=128;
    d(1,:) = fgets(fid);
    e(1,:)=fgets(fid);
    G=[];
    for k=1:1:keylen/4
        
        % f=hex2dec(e(k));
        g=dec2bin(hex2dec(e(k)));
        g1=str2num(g);
        %disp(fliplr(g1));
        g2=num2str(g1,'%04d');
        % fprintf("g2=%s\n",g2);
        g3=flipud(g2')';
        % fprintf("g3=%s\n",g3);
        %disp(g2);
        G=[g3,G];
        % fprintf("G=%s\n",G);
        clear g g1 g2 g3 
    end
    key1=[];
    for m=1:1:keylen
       key1(1,m)=str2double(G(1,m));
    end
    key=[key,key1'];
%     COUT(key1');

    %%���Ķ�ȡ
    clear f g;
    f(1,:)=fgets(fid);
    plaintextlen=128;
    G=[];
    for k=1:1:plaintextlen/4
        g=dec2bin(hex2dec(f(k)));
        g1=str2num(g);
        g2=num2str(g1,'%04d');
        g3=flipud(g2')';
        G=[g3,G];
    clear g g1 g2 g3
    end
    plaintext1=[];
    for m=1:1:plaintextlen
       plaintext1(1,m)=str2double(G(1,m));
    end
    plaintext=[plaintext,plaintext1'];
%     COUT(plaintext1');
    
    %%���Ķ�ȡ
    f1(1,:)=fgets(fid);
    f(1,:)=fgets(fid);
    cipherlen=128;
    G=[];
    for k=1:1:cipherlen/4
    g=dec2bin(hex2dec(f(k)));
    g1=str2num(g);
    g2=num2str(g1,'%04d');
    g3=flipud(g2')';
    G=[g3,G];
    clear g g1 g2 g3 e 
    end
    cipher1=[];
    for m=1:1:cipherlen
       cipher1(1,m)=str2double(G(1,m));
    end
    cipher=[cipher,cipher1'];
%     COUT(cipher1');
    
%     fprintf('k=%d\n',numtraces);
    


    % %�������ݵ���ȡ
    fileName1=['E:\',name,'#' num2str(numtraces) '#.csv'];   %����һ���ļ�λ��
     A=csvread(fileName1,0,0);%ֻ��һ��traces
    traces=[traces,A(:,1)];
    
    if(numtraces<=8)%��һ��ͨ�������⣬��Ҫ(����Ҫȥ��ǰ4������)
        key=[];
        cipher=[];
        plaintext=[];
        traces=[];
    end
end
fclose(fid);
% traces=traces(400:1600,:);

%trigger=CC';
fclose all;
save(['E:\����8����Ե�Ƽ� - SAKURA-G������ɲ����\',filetime,'.mat'],'traces','key','cipher','plaintext','-v7.3');
%clearvars -except coff cipher key traces trigger

% function COUT(data)
% %16����key��ʾ
%     n=size(data,1);
%     t=flipud(data);   
%     for ii=1:1:n/4
%          a=num2str(t(4*(ii-1)+1:ii*4))';
%          %fprintf("bin2dec=%d\n",bin2dec(a));
%          a=bin2dec(a);
%          a=dec2hex(a);
%          fprintf("%c",a);
%     end
%     fprintf("\n");
% end

